import ChartViewer from "src/shared/chart-viewer";

export default class Charts {

    private chartViewer: ChartViewer;
    private chartViewer2: ChartViewer;
    private chartViewer3: ChartViewer;
    private chartViewer4: ChartViewer;

    
    private Paint(Qname: string, Data: string[], Values: number[], ID: string, chartViewer: ChartViewer, type_bar: string, color_line: string, color_bag: string) {
        chartViewer = new ChartViewer(ID, {
            type: type_bar,
            data: {
                labels: Data,
                datasets: [{
                    label: Qname,
                    data: Values,
                }],
            },
            options: {
                scales: {
                    yAxes: [{
                        ticks: {
                            min: 0,
                        },
                    }],
                },
                elements: {
                    rectangle: {
                        backgroundColor: color_bag,
                    },
                    line: {
                        backgroundColor: color_line,
                    },
                },
            },
        });
    }

    private async GraphChart(url: string, id_chart: string, chart: ChartViewer, type_bar: string, color: string) {
        var result: { Qname: string; Data: string[]; Values: number[] };
        try {
            result = await $.getJSON(url);
        }
        catch (e) { }
        this.Paint(result.Qname, result.Data, result.Values, id_chart, chart, type_bar, color, color);

    }

    private async Graph()
	{
		var data1 = Object();
        var result: { Qname: string; Data: string[]; Values: number[] };
        		
        this.GraphChart('/home/queryrent', "chart-viewer", this.chartViewer, 'bar', 'gray');

        this.GraphChart('/home/querysale', "chart-viewer2", this.chartViewer2, 'horizontalBar', 'gray');

        this.GraphChart('/home/querymoney', "chart-viewer3", this.chartViewer3, 'bar', 'blue');

        this.GraphChart('/home/queryhouse', "chart-viewer4", this.chartViewer4, 'line', 'blue');

    }

    constructor(params?: { [key: string]: string }, queryString?: { [key: string]: string }) {
        this.Graph();
    }
}
