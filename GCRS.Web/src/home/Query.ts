import Feature from "ol/feature";
import LineString from "ol/geom/linestring";
import VectorLayer from "ol/layer/vector";
import MapBrowserEvent from "ol/mapbrowserevent";
import VectorSource from "ol/source/vector";
import Stroke from "ol/style/stroke";
import Style from "ol/style/style";
import MapViewer from "src/shared/map-viewer";
import Draw from "ol/interaction/draw";

import Geom from "ol/geom";
import Select from "ol/interaction/select";
import Collection from "ol/collection";
import Point from "ol/geom/point";

export default class Query {

    private mapViewer: MapViewer;
    private draw: Draw;
    private source: VectorSource;
    private pols:  Array<Array<Array<number>>>= Array<Array<Array<number>>>();
    private form1 = document.getElementById('houses-form') as HTMLFormElement;

    private Layer: VectorLayer;

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

        this.source = new VectorSource({ wrapX: false });
        this.mapViewer.map.addLayer(new VectorLayer({ source: this.source }));
        
        this.addInteraction();
        this.form1.addEventListener("submit", () => this.Send());

        this.Layer = new VectorLayer({
            source: new VectorSource(),
        });
        this.mapViewer.map.addLayer(this.Layer);
        this.F();

    }


    private async F() {
        //Calling a controler
        let url = `/map/getpublishingslocations`;

        let result: any;
        try {
            result = await $.getJSON(url);
        } catch (e) {
            alert(e.toString());
            return;
        }
        let sSource = this.Layer.getSource();
        for (const point of result) {
            const justpoint = new Feature(new Point([point.Long, point.Lat], "XY"));
            sSource.addFeature(justpoint);
        }
    }
    private Send()
    {
        var newenvio = document.createElement('input');
        newenvio.setAttribute('type', 'json');
        newenvio.setAttribute('value', JSON.stringify(this.pols));
        newenvio.setAttribute('name', 'poligons');        
        newenvio.setAttribute('id', 'poligons');
        this.form1.appendChild(newenvio);
        console.log($(this.form1.elements));
    }

    private addInteraction() {
        this.draw = new Draw({
            source: this.source,
            type: "Polygon" as Geom.GeometryType,
        });
        this.mapViewer.map.addInteraction(this.draw);

        this.draw.on("drawstart", (event: Draw.Event) => {
        }, this);

        this.draw.on("drawend", async (event: Draw.Event) => {
            const polygon = event.feature.getGeometry() as Geom.Polygon;
            var coordinates = polygon.getCoordinates()[0];
            this.pols.push(coordinates);
        });
    }
}
