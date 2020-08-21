import { Chart } from "chart.js";

export default class ChartViewer {
    private chart: Chart;

    constructor(ctx: string, options: Chart.ChartConfiguration) {
        this.chart = new Chart(ctx, options);
    }
}
