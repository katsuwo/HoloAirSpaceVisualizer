using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MapMenuHandler : MonoBehaviour {
	private TextMeshPro scaleTextMesh = null;
	private TextMeshPro sizeTextMesh = null;
	private TextMeshPro tiltTextMesh = null;

	[SerializeField] private GameObject scaleLabel;
	[SerializeField] private GameObject sizeLabel;
	[SerializeField] private GameObject tiltLabel;

	[SerializeField] private float scale_min = 7;
	[SerializeField] private float scale_max = 15;
	[SerializeField] private float size_min = 1;
	[SerializeField] private float size_max = 10;
	[SerializeField] private float tilt_min = 0;
	[SerializeField] private float tilt_max = 90;

	[Serializable]
	public class SliderEvent : UnityEvent<float> { }

	[SerializeField] private MapController mapController;
	[SerializeField] private GameObject mapParent;
	[SerializeField] private GameObject mapBoundingBox;
	[SerializeField] private List<GameObject> sliderThumbs;

	private int mapScale = 0;
	private float mapSize = 0;
	private int mapTilt = 0;
	private int oldMapAreaSizeValue = 0;
	private GameObject mapAreaIndicator;
	private float mapAreaDisapearTimer = 0;
	private Util util;
	
	void Start() {
		mapAreaIndicator = GameObject.Find("MapArea");
		if (mapAreaIndicator == null) return;
		if (scaleLabel == null) return;
		if (sizeLabel == null) return;
		if (tiltLabel == null) return;
		if (scaleTextMesh == null) scaleTextMesh = scaleLabel.GetComponent<TextMeshPro>();
		if (sizeTextMesh == null) sizeTextMesh = sizeLabel.GetComponent<TextMeshPro>();
		if (tiltTextMesh == null) tiltTextMesh = tiltLabel.GetComponent<TextMeshPro>();
		mapAreaIndicator.SetActive(false);

		SolverHandler handler = mapParent.GetComponent<SolverHandler>();
		HandConstraint handConstraint = mapParent.GetComponent<HandConstraint>();
		BoundsControl boundingBox = mapBoundingBox.GetComponent<BoundsControl>();
		ObjectManipulator manipulator = mapBoundingBox.GetComponent<ObjectManipulator>();
		if (handler != null) handler.enabled = false;
		if (handConstraint != null) handConstraint.enabled = false;
		mapBoundingBox.SetActive(false);
	}

	void Update() {
		if (mapAreaDisapearTimer > 0.0f) {
			mapAreaDisapearTimer -= Time.deltaTime;
			mapAreaIndicator.SetActive(true);
			if (mapAreaDisapearTimer <= 0.0f) {
				mapAreaIndicator.SetActive(false);
			}
		}
	}

	public void OnScaleSliderUpdated(SliderEventData eventData) {
		var diff = scale_max - scale_min;
		var val = (int) (eventData.NewValue * diff + scale_min);
		mapScale = val;

		if (scaleTextMesh == null) return;
		scaleTextMesh.text = $"{val:D}";
	}

	//0~1.0で得られるスライダー値を
	// 0.2 0.3 0.4.....1.0 2.0 3.0 ...... sizeMAXに正規化し、mapsizeを決定する
	//スライダーの下半分を0.1 ~ 1.0に割り当てる
	public void OnSizeSliderUpdated(SliderEventData eventData) {
		mapAreaDisapearTimer = 3.0f;

		if (mapAreaIndicator == null) return;
		mapAreaIndicator.SetActive(true);
		if (sizeTextMesh == null) return;

		if (eventData.NewValue <= 0.5f) {
			var ratio = eventData.NewValue / 0.5f;
			mapSize = Mathf.Floor(ratio * 10f) / 10f;
			if (mapSize <= 0.2f) mapSize = 0.2f;
		}
		else {
			var ratio = (eventData.NewValue - 0.5f) * 2f;
			mapSize = Mathf.Ceil(ratio * size_max);
		}
		sizeTextMesh.text = $"{mapSize:F1}";
		MapAreaUpdate(mapSize);
	}

	public void OnTiltSliderUpdated(SliderEventData eventData) {
		var diff = tilt_max - tilt_min;
		var val = (int) (eventData.NewValue * diff + tilt_min);
		mapTilt = val;

		if (tiltTextMesh == null) return;
		tiltTextMesh.text = $"{val:D}";

		if (mapController == null) return;
		mapController.MapTiltUpdate(mapTilt);
	}
	
	private void MapAreaUpdate(float scale) {
		if (mapAreaIndicator == null) return;
		if (mapBoundingBox == null) return;
		mapAreaIndicator.transform.localScale = new Vector3(scale, 1, scale);
		mapAreaIndicator.SetActive(true);
	}

	public void OnSetButtonPressed() {
		if (mapController == null) return;
		var currentMapHeight = mapController.GetHeightFromCoordinate(mapController.centerCoord);
		mapController.MapExtentUpdate(mapSize);
		mapController.MapZoomUpdate(mapScale);
		mapController.adjustMapHeight(currentMapHeight);
		
		var handler = mapParent.GetComponent<SolverHandler>();
		var handConstraint = mapParent.GetComponent<HandConstraint>();
		if (handler == null || handConstraint == null || mapBoundingBox == null) return;
		handler.enabled = false;
		handConstraint.enabled = false;
		mapBoundingBox.SetActive(false);
	}

	public void OnMoveButtonPressed() {
		if (mapParent == null) return;

		var handler = mapParent.GetComponent<SolverHandler>();
		var handConstraint = mapParent.GetComponent<HandConstraint>();
		var mapRotation = mapParent.transform.rotation;

		if (handler == null || handConstraint == null || mapBoundingBox == null) return;

		//Moveを押した瞬間に手にMAPを吸い寄せる
		handler.enabled = true;
		handConstraint.enabled = true;
		mapBoundingBox.SetActive(true);

		StartCoroutine(Util.DelayMethod(10,
			() => {
				//押下から一定時間経過後、手を吸い付く動作を停止	
				if (mapController.currentZoom >= 2.0) mapController.MapExtentUpdate(1.0f);
				handler.enabled = false;
				handConstraint.enabled = false;
				mapParent.transform.rotation = mapRotation;
			}
		));
	}

	//how to find height of a point using lat long to place object on map, Conversions.GeoToWorldPosition gives x and z values need y value.
	//https://github.com/mapbox/mapbox-unity-sdk/issues/222

	private IEnumerator DelayMethod(int delayFrameCount, Action action) {
		for (var i = 0; i < delayFrameCount; i++) yield return null;
		action();
	}

	//旧バージョンMRTKでスライダーつまみが軸からズレる対策。
	public void adjustSliderThumbs() {
		foreach (var thumb in sliderThumbs) {
			var pos = thumb.transform.localPosition;
			thumb.transform.localPosition = new Vector3(0, pos.y, pos.z);
		}
	}
}