import Feature from "ol/feature";
import LineString from "ol/geom/linestring";
import VectorLayer from "ol/layer/vector";
import MapBrowserEvent from "ol/mapbrowserevent";
import VectorSource from "ol/source/vector";
import Stroke from "ol/style/stroke";
import Fill from "ol/style/fill";
import Style from "ol/style/style";
import MapViewer from "src/shared/map-viewer";
import Point from "ol/geom/point";

export default class RouteMin {

    private mapViewer: MapViewer;
    private pathLayer: VectorLayer;

    constructor() {
        this.mapViewer = new MapViewer({
            element: "map-viewer",
            layers: [
                "cuba",
                "landuse",
                "roads",
                "water",
                "waterways",
            ],
        });
        this.pathLayer = new VectorLayer({
            source: new VectorSource(),
        });
        this.mapViewer.map.addLayer(this.pathLayer);
        this.F();
        
    }

    private async F()
    {
        //Calling a controler
        let url = `/map/getroutepoints`;

        let result: { PathNodes: ILatLon[] };
        try {
            result = await $.getJSON(url);
        } catch (e) {
            alert(e.toString());
            return;
        }

        const pathSource = this.pathLayer.getSource();
        const high = false;
        const lineStyle = new Style({
            stroke: new Stroke({
                color: high ? "orange" : [17, 152, 207, 1],
                width: high ? 7 : 5,
            }),
            zIndex: -1,
        });
        const overlineStyle = new Style({
            stroke: new Stroke({
                color: "white",
                width: high ? 10 : 8,
            }),
            zIndex: -2,
        });
        const boderLine = new Style({
            stroke: new Stroke({
                color: "lightgray",
                width: high ? 11 : 9,
            }),
            zIndex: -3,
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
        for (const point of result.PathNodes) {
            const justpoint = new Feature(new Point([point.Longitude, point.Latitude], "XY"));
            pathSource.addFeature(justpoint);
        }
    }
}
