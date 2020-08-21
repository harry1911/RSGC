import ChartViewer from "src/shared/chart-viewer";

export default class About {

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
		
		

        const provinces: { [key: string]: IChartData } = {
            habana: {
                labels: ["Playa", "Habana Vieja", "Centro Habana", "Plaza"],
                data: [12, 19, 3, 5],
            },
            matanzas: {
                labels: ["Matanzas", "Varadero", "Cárdenas"],
                data: [7, 24, 11],
            },
            pinardelrio: {
                labels: ["Pinar del Río", "Viñales"],
                data: [5, 9],
            },
        };
		
        let data: IChartData;
        if (params != null && params.id != null) {
            data = provinces[params.id.toLowerCase()];
        } else {
            for (const key of Object.getOwnPropertyNames(provinces)) {
                if (data == null) {
                    data = provinces[key];
                } else {
                    data.labels = data.labels.concat(provinces[key].labels);
                    data.data = data.data.concat(provinces[key].data);
                }
            }
        }

        if (data == null) {
            return;
        }

		this.Graph();
    }
}
