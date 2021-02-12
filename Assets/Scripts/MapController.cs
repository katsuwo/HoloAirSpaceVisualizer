using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mapbox.Map;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;

public class MapController : MonoBehaviour {
	private AbstractMap map;
	[SerializeField] private float initial_Y = -1.0f;
	[SerializeField] private GameObject mapAreaIndicator;
	[SerializeField] private GameObject mapBoundingBox;

	public float currentZoom {
		get => map.Zoom;
		set => MapZoomUpdate(value);
	}

	public Vector2d centerCoord {
		get => map.CenterLatitudeLongitude;
	}

	private Vector2d mapCenter = new Vector2d(35.3611236, 138.7266352);

	// Start is called before the first frame update
	private void Start() {
		map = gameObject.GetComponent<AbstractMap>();
		if (map == null) return;

		var mapInitialPosition = new Vector3(0, initial_Y, 0);
		map.transform.parent.localPosition = mapInitialPosition;

		map.MapVisualizer.OnMapVisualizerStateChanged += (ModuleState s) => {
			switch (s) {
				case ModuleState.Initialized:
					Debug.Log("State Initialized");
					break;
				case ModuleState.Working:
					Debug.Log("State Working");
					break;
				case ModuleState.Finished:
					Debug.Log("State Finished");
					adjustMapHeight(initial_Y);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(s), s, null);
			}
		};

		MapExtentUpdate(1f);
		MapZoomUpdate(12);
	}

	// Update is called once per frame
	private void Update() {
		initial_Y = map.transform.parent.transform.localPosition.y;
	}

	public void MapTiltUpdate(float tilt) {
		if (map == null) return;
		var temp = map.transform.rotation.eulerAngles;
		map.transform.eulerAngles = new Vector3(-tilt, temp.y, temp.z);
	}

	public void MapExtentUpdate(float val) {
		if (map == null) return;

		if (val <= 1.0) {
			var extent = 0f;
			map.SetExtentOptions(new RangeTileProviderOptions
				{west = (int) extent, east = (int) extent, north = (int) extent, south = (int) extent});
			map.transform.parent.transform.localScale = new Vector3(val, val, val);
			mapBoundingBox.transform.localScale = new Vector3(val, val, val);
		}
		else {
			var extent = val - 1.0f;
			map.SetExtentOptions(new RangeTileProviderOptions
				{west = (int) extent, east = (int) extent, north = (int) extent, south = (int) extent});
			map.transform.parent.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			mapBoundingBox.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
		}
	}

	public void MapZoomUpdate(float zoom) {
		if (map == null) return;
		var currentZoom = map.AbsoluteZoom;
		if ((int) zoom == currentZoom) return;

		map.Initialize(mapCenter, (int) zoom);
		Debug.Log("Map Zoom updated");
	}

	public float GetHeightFromCoordinate(Vector2d coordinate) {
		double lat = coordinate.x;
		double lon = coordinate.y;
		//get tile ID
		var tileIDUnwrapped = TileCover.CoordinateToTileId(new Mapbox.Utils.Vector2d(lat, lon), (int) map.Zoom);

		//get tile
		UnityTile tile = map.MapVisualizer.GetUnityTileFromUnwrappedTileId(tileIDUnwrapped);

		//lat lon to meters because the tiles rect is also in meters
		Vector2d v2d = Conversions.LatLonToMeters(new Mapbox.Utils.Vector2d(lat, lon));

		//get the origin of the tile in meters
		Vector2d v2dcenter = tile.Rect.Center - new Mapbox.Utils.Vector2d(tile.Rect.Size.x / 2, tile.Rect.Size.y / 2);

		//offset between the tile origin and the lat lon point
		Vector2d diff = v2d - v2dcenter;

		//maping the diffetences to (0-1)
		float Dx = (float) (diff.x / tile.Rect.Size.x);
		float Dy = (float) (diff.y / tile.Rect.Size.y);

		//height in unity units
		var height = tile.QueryHeightData(Dx, Dy);
		Debug.Log($"Height :{height:F2}");
		return height;
	}

	public void adjustMapHeight(float targetHeight) {
		var centerHeight = GetHeightFromCoordinate(map.CenterLatitudeLongitude);
		var diff = targetHeight - centerHeight;
		var currentMapPosition = map.transform.parent.localPosition;
		map.transform.parent.localPosition = new Vector3(currentMapPosition.x, diff, currentMapPosition.z);
	}
}