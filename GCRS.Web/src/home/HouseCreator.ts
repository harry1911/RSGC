import Feature from "ol/feature";
//import LineString from "ol/geom/linestring";
import VectorLayer from "ol/layer/vector";
import MapBrowserEvent from "ol/mapbrowserevent";
import VectorSource from "ol/source/vector";
import Stroke from "ol/style/stroke";
import Style from "ol/style/style";
import MapViewer from "src/shared/map-viewer";
import Point from "ol/geom/point";

export default class HouseCreator {

    private mapViewer: MapViewer;
    private pointLayer: VectorLayer;
    private result: Feature;
    private coordinates: any;

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
            onClick: this.onClick.bind(this),
        });
        this.pointLayer = new VectorLayer({
            source: new VectorSource(),
        });
        this.mapViewer.map.addLayer(this.pointLayer);
        this.coordinates = document.getElementById("Coordinates");
    }

    private async onClick(e: MapBrowserEvent) {
        if (this.result != null) {
            const pointSource = this.pointLayer.getSource();
            pointSource.removeFeature(this.result);
        }
        var x = e.coordinate[0];
        var y = e.coordinate[1];
        this.result = new Feature(new Point(
            [x,y], "XY"));
        const pointSource = this.pointLayer.getSource();
        pointSource.addFeature(this.result);
        this.coordinates.value = x.toString() + " " + y.toString();
    }
}
