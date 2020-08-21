import Image from "ol/layer/image";
import Map from "ol/map";
import MapBrowserEvent from "ol/mapbrowserevent";
import Overlay from "ol/overlay";
import proj from "ol/proj";
import ImageWMS from "ol/source/imagewms";
import View from "ol/view";

interface IMapViewerOptions {
    /**
     * Target element where map its renderer, can be a html element or a identifier for an html element
     */
    element: Element | string;
    /** List of available layers from server
     */
    layers: MapLayerTypes[];
    /**
     * Each component is attached on a single location on map.
     * An Overlay can have a html element, like an image, button, etc.
     */
    overlays?: Overlay[];
    /**
     * Center of the map
     */
    center?: ol.Coordinate;
    /**
     * Zoom level
     */
    zoom?: number;
    /**
     * Event handler for click on map
     */
    onClick?: (evt: MapBrowserEvent) => void;
}

export default class MapViewer {
    public map: Map;

    constructor(private options: IMapViewerOptions) {
        this.map = new Map({
            target: this.options.element,
            layers: [
                new Image({
                    source: new ImageWMS({
                        url: "/map",
                        projection: proj.get("EPSG:4326"),
                        params: {
                            LAYERS: this.options.layers.map((e) => e.toString()).join(","),
                        },
                    }),
                }),
            ],
            view: new View({
                projection: proj.get("EPSG:4326"),
                center: this.options.center || [-82.38653898239136, 23.122648000717167],
                zoom: this.options.zoom || 14,
            }),
            overlays: this.options.overlays,
        });
        if (this.options.onClick) {
            this.map.on("click", (evt: MapBrowserEvent) => this.options.onClick(evt));
        }
    }
}
