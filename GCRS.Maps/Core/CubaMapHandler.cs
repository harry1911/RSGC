using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Rendering.Decoration;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;
using SharpMap.Web.Wms;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Text;

namespace GCRS.Maps.Core
{
    public class CubaMapHandler
    {
        public string ShapeDir { get; set; }

        public CubaMapHandler(string shapeDir)
        {
            ShapeDir = shapeDir;
        }

        /// <summary>
        ///     Prepares the response.
        /// </summary>
        public void ProcessRequest(string url)
        {
            var map = GetMap();
            var description = GetDescription(url);
            WmsServer.ParseQueryString(map, description);
        }

        /// <summary>
        ///     Gets the description
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private Capabilities.WmsServiceDescription GetDescription(string url)
        {
            var description = new Capabilities.WmsServiceDescription("Map Server", url)
            {
                MaxWidth = 0,
                MaxHeight = 0,
                Abstract = "Map Server.",
                Keywords = new[] { "cuba", "roads", "poi" }
            };
            return description;
        }

        /// <summary>
        ///     Gets the current map.
        /// </summary>
        /// <returns>The map.</returns>
        private SharpMap.Map GetMap()
        {
            var map = new SharpMap.Map(new Size(1, 1)) { BackColor = Color.FromArgb(255, 246, 243, 236) };
            var disclaimer = new Disclaimer
            {
                Font = new Font("Arial", 7f, FontStyle.Italic),
                Text = "Geodata from OpenStreetMap (CC-by-SA)\nTransformed to Shapefile by geofabrik.de",
                Anchor = MapDecorationAnchor.CenterBottom
            };
            map.Decorations.Add(disclaimer);
            var encoding = Encoding.UTF8;
            var transparentStyle = new VectorStyle
            {
                Fill = Brushes.Transparent,
                EnableOutline = true,
                Line = { Brush = Brushes.Transparent },
                Outline = { Brush = Brushes.Transparent },
                Symbol = null
            };

            #region cuba

            var cubaLayer = new VectorLayer("cuba",
                new ShapeFile(Path.Combine(ShapeDir, "admin.shp"), true, false, 4326) { Encoding = encoding })
            {
                Style = new VectorStyle
                {
                    EnableOutline = true,
                    Fill = Brushes.FloralWhite
                },
                SmoothingMode = SmoothingMode.AntiAlias,
                ClippingEnabled = true
            };
            map.Layers.Add(cubaLayer);

            #endregion

            #region landuse layer

            var landuseStyle = transparentStyle.Clone();
            landuseStyle.Fill = new SolidBrush(Color.FromArgb(255, 246, 243, 236));
            var landuseLayer = new VectorLayer("landuse",
                new ShapeFile(Path.Combine(ShapeDir, "gis.osm_landuse_a_free_1.shp"), true, false, 4326) { Encoding = encoding })
            {
                Style = landuseStyle,
                SmoothingMode = SmoothingMode.AntiAlias,
                ClippingEnabled = true
            };
            var landuseTheme = new CustomTheme(row =>
            {
                var caseVal = (string)row["fclass"];
                caseVal = caseVal.ToLowerInvariant();
                var returnStyle = landuseStyle.Clone();

                switch (caseVal)
                {
                    case "forest":
                        returnStyle.Fill = new SolidBrush(Color.FromArgb(255, 221, 230, 213));
                        returnStyle.Outline.Brush = new SolidBrush(Color.FromArgb(255, 221, 230, 213));
                        break;
                    case "park":
                    case "grass":
                    case "national_park":
                        returnStyle.Fill = new SolidBrush(Color.FromArgb(255, 200, 225, 175));
                        returnStyle.Outline.Brush = new SolidBrush(Color.FromArgb(255, 200, 225, 175));
                        break;
                    case "residential":
                    case "commercial":
                    case "retail":
                        returnStyle.Fill = new SolidBrush(Color.FromArgb(255, 231, 227, 216));
                        returnStyle.Outline.Brush = new SolidBrush(Color.FromArgb(255, 231, 227, 216));
                        break;
                    case "industrial":
                        returnStyle.Fill = new SolidBrush(Color.FromArgb(255, 221, 216, 210));
                        returnStyle.Outline.Brush = new SolidBrush(Color.FromArgb(255, 221, 216, 210));
                        break;
                    case "military":
                        returnStyle.Fill = new SolidBrush(Color.FromArgb(255, 221, 220, 215));
                        returnStyle.Outline.Brush = new SolidBrush(Color.FromArgb(255, 221, 220, 215));
                        break;
                    case "wood":
                    case "nature_reserve":
                    case "farm":
                    case "cemetery":
                    case "water":
                    case "riverbank":
                    case "allotments":
                    case "meadow":
                    case "recreation_ground":
                    case "quarry":
                    case "orchard":
                    case "vineyard":
                    case "heath":
                    case "scrub":
                        returnStyle = null;
                        break;
                    default:
                        returnStyle = null;
                        break;
                }
                return returnStyle;
            });
            landuseLayer.Theme = landuseTheme;
            map.Layers.Add(landuseLayer);

            #endregion

            #region road layer

            var roadsLayer = new VectorLayer("roads",
                new ShapeFile(Path.Combine(ShapeDir, "gis.osm_roads_free_1.shp"), true, false, 4326) { Encoding = encoding })
            {
                SmoothingMode = SmoothingMode.AntiAlias,
                ClippingEnabled = true,
                Style = transparentStyle
            };
            roadsLayer.DataSource.Open();
            var roadsExtents = roadsLayer.DataSource.GetExtents();
            var themeRoads = new CustomTheme(row =>
            {
                var returnStyle = new VectorStyle
                {
                    Line = new Pen(new SolidBrush(Color.FromArgb(255, 240, 240, 160))),
                    EnableOutline = true,
                    Outline = new Pen(new SolidBrush(Color.Black), 1)
                };

                switch ((string)row["fclass"])
                {
                    case "rail":
                        returnStyle.Fill = Brushes.White;
                        returnStyle.Line.DashPattern = new[] { 4f, 4f };
                        returnStyle.Line.Width = 4;
                        returnStyle.EnableOutline = true;
                        returnStyle.Outline.Brush = Brushes.Black;
                        returnStyle.Outline.Width = 6;
                        break;
                    case "canal":
                        returnStyle.Fill = Brushes.Aqua;
                        returnStyle.Outline.Brush = Brushes.DarkBlue;
                        returnStyle.Outline.Width = 5;
                        break;
                    case "cycleway":
                    case "footway":
                    case "pedestrian":
                        returnStyle.Line.Brush = Brushes.DarkGray;
                        returnStyle.Line.DashStyle =
                            DashStyle.Dot;
                        returnStyle.Line.Width = 1;
                        returnStyle.MaxVisible = roadsExtents.Width * 0.025d;
                        break;
                    case "living_street":
                    case "residential":
                        returnStyle.Line.Brush = Brushes.LightGoldenrodYellow;
                        returnStyle.Line.Width = 2;
                        returnStyle.EnableOutline = true;
                        returnStyle.Outline.Brush = Brushes.DarkGray;
                        returnStyle.Outline.Width = 4;
                        returnStyle.MaxVisible = roadsExtents.Width * 0.075d;
                        break;
                    case "primary":
                        returnStyle.Line.Brush = Brushes.LightGoldenrodYellow;
                        returnStyle.Line.Width = 7;
                        returnStyle.EnableOutline = true;
                        returnStyle.Outline.Brush = Brushes.Black;
                        returnStyle.Outline.Width = 11;
                        break;
                    case "secondary":
                        returnStyle.Line.Brush = Brushes.LightGoldenrodYellow;
                        returnStyle.Line.Width = 6;
                        returnStyle.EnableOutline = true;
                        returnStyle.Outline.Brush = Brushes.Black;
                        returnStyle.MaxVisible = roadsExtents.Width * 0.15d;
                        returnStyle.Outline.Width = 10;
                        break;
                    case "tertiary":
                        returnStyle.Line.Brush = Brushes.LightGoldenrodYellow;
                        returnStyle.Line.Width = 5;
                        returnStyle.EnableOutline = true;
                        returnStyle.Outline.Brush = Brushes.Black;
                        returnStyle.MaxVisible = roadsExtents.Width * 0.3d;
                        returnStyle.Outline.Width = 9;
                        break;
                    case "path":
                    case "track":
                    case "unclassified":
                        returnStyle.Line.Brush = Brushes.DarkGray;
                        returnStyle.Line.DashStyle =
                            DashStyle.DashDotDot;
                        returnStyle.Line.Width = 1;
                        returnStyle.MaxVisible = roadsExtents.Width * 0.0125d;
                        break;
                    default:
                        returnStyle = null;
                        break;
                }
                return returnStyle;
            });
            roadsLayer.Theme = themeRoads;
            map.Layers.Add(roadsLayer);

            #endregion

            #region water layer

            var waterLayer = new VectorLayer("water",
                new ShapeFile(Path.Combine(ShapeDir, "gis.osm_water_a_free_1.shp"), true, false, 4326) { Encoding = encoding })
            {
                Style = transparentStyle.Clone(),
                SmoothingMode = SmoothingMode.AntiAlias,
                ClippingEnabled = true
            };
            waterLayer.DataSource.Open();
            var waterExtents = waterLayer.DataSource.GetExtents();
            var waterTheme = new CustomTheme(row =>
            {
                var returnStyle = new VectorStyle
                {
                    EnableOutline = true,
                    MaxVisible = waterExtents.Width * 0.05
                };
                switch ((string)row["fclass"])
                {
                    case "water":
                        returnStyle.Fill = new SolidBrush(Color.FromArgb(255, 165, 190, 220));
                        returnStyle.Outline.Brush = new SolidBrush(Color.FromArgb(255, 165, 190, 220));
                        break;
                    case "river":
                        returnStyle.Fill = new SolidBrush(Color.FromArgb(255, 165, 190, 220));
                        returnStyle.Outline.Brush = new SolidBrush(Color.FromArgb(255, 165, 190, 220));
                        returnStyle.Outline.Width = 2;
                        break;
                    default:
                        returnStyle = null;
                        break;
                }
                return returnStyle;
            });
            waterLayer.Theme = waterTheme;
            map.Layers.Add(waterLayer);

            #endregion

            #region waterways layer

            var waterwaysLayer = new VectorLayer("waterways",
                new ShapeFile(Path.Combine(ShapeDir, "gis.osm_waterways_free_1.shp"), true, false, 4326) { Encoding = encoding })
            {
                Style = transparentStyle.Clone(),
                SmoothingMode = SmoothingMode.AntiAlias,
                ClippingEnabled = true
            };
            waterwaysLayer.DataSource.Open();
            var waterwaysExtents = waterwaysLayer.DataSource.GetExtents();
            var waterwaysTheme = new CustomTheme(row =>
            {
                var returnStyle = new VectorStyle
                {
                    EnableOutline = false,
                    MaxVisible = waterwaysExtents.Width * 0.05
                };
                switch ((string)row["fclass"])
                {
                    case "river":
                        returnStyle.Line.Brush = new SolidBrush(Color.FromArgb(255, 165, 190, 220));
                        returnStyle.Line.Width = 2;
                        break;
                    default:
                        returnStyle = null;
                        break;
                }
                return returnStyle;
            });
            waterwaysLayer.Theme = waterwaysTheme;
            map.Layers.Add(waterwaysLayer);

            #endregion

            #region points

            var pointsLayer = new VectorLayer("points",
                new ShapeFile(Path.Combine(ShapeDir, "gis.osm_pois_free_1.shp"), true, false, 4326) { Encoding = encoding })
            {
                Style = transparentStyle.Clone()
            };
            pointsLayer.DataSource.Open();
            var pointsExtents = pointsLayer.DataSource.GetExtents();
            var pointsTheme = new CustomTheme(row =>
            {
                var returnStyle = new VectorStyle
                {
                    SymbolScale = 0.3f,
                    MaxVisible = pointsExtents.Width * 0.0125d
                };
                switch ((string)row["fclass"])
                {
                    case "bank":
                        returnStyle.Symbol = GetBitmap("bank");
                        break;
                    case "hospital":
                        returnStyle.Symbol = GetBitmap("hospital-o");
                        break;
                    case "bar":
                        returnStyle.Symbol = GetBitmap("beer");
                        break;
                    case "embassy":
                        returnStyle.Symbol = GetBitmap("town");
                        break;
                    case "parking":
                        returnStyle.Symbol = GetBitmap("parking");
                        break;
                    case "pub":
                        returnStyle.Symbol = GetBitmap("restaurant");
                        break;
                    case "cinema":
                        returnStyle.Symbol = GetBitmap("cinema");
                        break;
                    case "zoo":
                        returnStyle.Symbol = GetBitmap("zoo");
                        break;
                    case "theatre":
                        returnStyle.Symbol = GetBitmap("theatre");
                        break;
                    case "monument":
                    case "memorial":
                        returnStyle.Symbol = GetBitmap("monument");
                        break;
                    case "restaurant":
                        returnStyle.Symbol = GetBitmap("restaurant");
                        break;
                    case "sport":
                        returnStyle.Symbol = GetBitmap("pitch");
                        break;
                    case "airport":
                        returnStyle.Symbol = GetBitmap("airport");
                        break;
                    case "college":
                    case "university":
                        returnStyle.Symbol = GetBitmap("college");
                        break;
                    case "cafe":
                        returnStyle.Symbol = GetBitmap("cafe");
                        break;
                    case "fire":
                        returnStyle.Symbol = GetBitmap("fire");
                        break;
                    case "police":
                        returnStyle.Symbol = GetBitmap("police");
                        break;
                    case "fuel":
                        returnStyle.Symbol = GetBitmap("fuel");
                        break;
                    case "nightclub":
                        returnStyle.Symbol = GetBitmap("music");
                        break;
                    case "museum":
                        returnStyle.Symbol = GetBitmap("museum");
                        break;
                    case "hotel":
                    case "hostel":
                    case "guesthouse":
                        returnStyle.Symbol = GetBitmap("hotel");
                        break;
                    case "fast_food":
                        returnStyle.Symbol = GetBitmap("food");
                        break;
                    case "pharmacy":
                        returnStyle.Symbol = GetBitmap("pharmacy");
                        break;
                    default:
                        returnStyle.Symbol = GetBitmap("map-marker");
                        break;
                }
                return returnStyle;
            });
            pointsLayer.Theme = pointsTheme;
            map.Layers.Add(pointsLayer);

            #endregion

            #region Houses

            var HousesLayer = new VectorLayer("houses",
                new SqlServer2008(ConfigurationManager.ConnectionStrings["DB"].ConnectionString,"dbo.Houses","Location","HouseID",SqlServerSpatialObjectType.Geometry,true,4326))
            {       
                Style = new VectorStyle {SymbolScale=0.3f }
            };
            map.Layers.Add(HousesLayer);
            #endregion

            return map;
        }

        private IDictionary<string, Stream> Cache { get; set; } = new Dictionary<string, Stream>();

        private Bitmap GetBitmap(string icon)
        {
            if (Cache.ContainsKey(icon))
            {
                var stream = Cache[icon];
                return new Bitmap(stream);
            }
            else
            {
                var mapAssembly = Assembly.GetAssembly(typeof(CubaMapHandler));
                var stream = mapAssembly.GetManifestResourceStream($"{nameof(GCRS)}.{nameof(Maps)}.Images.{icon}.png");
                Cache.Add(icon, stream);
                return new Bitmap(stream);
            }
        }
    }
}