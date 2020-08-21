import { ConfigurableRoute, RouteRecognizer } from "aurelia-route-recognizer";
import { parse } from "qs";

interface IObjectConstructor {
    new(routeData?: object, queryString?: object): object;
}

interface IRouteConfig {
    path: string[];
    component: IObjectConstructor;
}

interface IRouterConfig {
    routes: IRouteConfig[];
}

export default class Router {
    private recognizer = new RouteRecognizer();

    constructor(private configs: IRouterConfig) {
        const configRoutes: ConfigurableRoute[] = [];
        for (const config of configs.routes) {
            for (const route of config.path) {
                configRoutes.push({
                    path: route,
                    caseSensitive: false,
                    handler: { name: config.component.toString() },
                });
            }
        }
        this.recognizer.add(configRoutes);
    }

    public start() {
        const routes = this.recognizer.recognize(window.location.pathname.toLowerCase());
        if (routes != null && routes.length > 0) {
            const qs = window.location.search == null || window.location.search === ""
                ? null
                : parse(window.location.search.substr(1));
            const componentType = this.configs.routes.find((e) => e.component.toString() === routes[0].handler.name);
            const component = new componentType.component(routes[0].params, qs);
        }
    }
}
