import Feature from "ol/feature";
import LineString from "ol/geom/linestring";
import VectorLayer from "ol/layer/vector";
import MapBrowserEvent from "ol/mapbrowserevent";
import VectorSource from "ol/source/vector";
import Stroke from "ol/style/stroke";
import Style from "ol/style/style";
import MapViewer from "src/shared/map-viewer";

export default class Home {

    private mapViewer: MapViewer;
    private firstClick: ol.Coordinate;
    private pathLayer: VectorLayer;

    constructor() {
        this.mapViewer = new MapViewer({
            element: "map-viewer",
            layers: [
                "cuba",
                "landuse",
                "points",
                "roads",
                "water",
                "waterways",
            ],
            onClick: this.onClick.bind(this),
        });
        this.pathLayer = new VectorLayer({
            source: new VectorSource(),
        });
        this.mapViewer.map.addLayer(this.pathLayer);
    }

    private async onClick(e: MapBrowserEvent) {
        if (this.firstClick) {
            const url = `/map/getroute?startX=${this.firstClick[0]}&startY=${this.firstClick[1]}
                    &endX=${e.coordinate[0]}&endY=${e.coordinate[1]}`;

            let result: { PathNodes: ILatLon[] };
            try {
                result = await $.getJSON(url);
            } catch (e) {
                alert(e.toString());
                return;
            }
            const pathSource = this.pathLayer.getSource();
            const high = true;
            const lineStyle = new Style({
                stroke: new Stroke({
                    color: high ? "orange" : [17, 152, 207, 1],
                    width: high ? 7 : 5,
                }),
                zIndex: 3,
            });
            const overlineStyle = new Style({
                stroke: new Stroke({
                    color: "white",
                    width: high ? 10 : 8,
                }),
                zIndex: 2,
            });
            const boderLine = new Style({
                stroke: new Stroke({
                    color: "lightgray",
                    width: high ? 11 : 9,
                }),
                zIndex: 1,
            });
            let previousPoint: ILatLon;
            for (const point of result.PathNodes) {
                if (previousPoint != null) {
                    const lineFeature = new Feature(new LineString(
                        [
                            [previousPoint.Longitude, previousPoint.Latitude],
                            [point.Longitude, point.Latitude],
                        ], "XY"),
                    );
                    lineFeature.setStyle([lineStyle, overlineStyle, boderLine]);
                    pathSource.addFeature(lineFeature);
                }
                previousPoint = {
                    Latitude: point.Latitude,
                    Longitude: point.Longitude,
                };
            }
        }
        this.firstClick = e.coordinate;
    }
}
