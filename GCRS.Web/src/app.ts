//import About from "./home/about";
//import Home from "./home/home";
import Chart from "./home/Charts"
import HouseCrator from "./home/HouseCreator"
import Query from "./home/Query"
import RouteMin from "./home/RouteMin"
import RouteMinRent from "./home/RouteMinRent"
import Router from "./shared/router"

const router = new Router({
    routes: [
        {
            path: ["home/charts/:id?"],
            component: Chart,
        },
        {
            path: ["Houses/Create"],
            component: HouseCrator,
        },
        {
            path: ["Query/Index"],
            component: Query,
        },
        {
            path: ["Query"],
            component: Query,
        },
        {
            path: ["Query/IndexPost"],
            component: Query,
        },
        {
            path: ["RequestSalePublishings/ListAttendedRequest"],
            component: RouteMin,
        },
        {
            path: ["RequestRentPublishings/ListAttendedRequest"],
            component: RouteMinRent,
        },
    ],
});
router.start();
