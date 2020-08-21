declare type MapLayerTypes = 'cuba' | 'landuse' | 'roads' | 'water' | 'waterways' | 'points' | 'houses' ;

interface ILatLon {
    Latitude: number;
    Longitude: number;
}

interface IChartData {
    labels: string[];
    data: number[];
}

declare module "ol/map" {
    export default ol.Map;
}

declare module "ol/view" {
    export default ol.View;
}

declare module "ol/layer/image" {
    export default ol.layer.Image;
}

declare module "ol/layer/vector" {
    export default ol.layer.Vector;
}

declare module "ol/source/imagewms" {
    export default ol.source.ImageWMS;
}

declare module "ol/source/vector" {
    export default ol.source.Vector;
}

declare module "ol/overlay" {
    export default ol.Overlay;
}

declare module "ol/mapbrowserevent" {
    export default ol.MapBrowserEvent;
}

declare module "ol/proj" {
    export default ol.proj;
}

declare module "ol/feature" {
    export default ol.Feature;
}

declare module "ol/style/style" {
    export default ol.style.Style;
}

declare module "ol/style/stroke" {
    export default ol.style.Stroke;
}

declare module "ol/geom/linestring" {
    export default ol.geom.LineString;
}

declare module "ol/geom/point" {
    export default ol.geom.Point;
}

declare module "ol/interaction/draw" {
    export default ol.interaction.Draw;
}

declare module "ol/geom" {
    export default ol.geom;
}

declare module "ol/interaction/select" {
    export default ol.interaction.Select;
}

declare module "ol/collection" {
    export default ol.Collection;
}

declare module "ol/style/fill" {
    export default ol.style.Fill;
}