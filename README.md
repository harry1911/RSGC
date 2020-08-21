# Introducción
El esqueleto de aplicación **G**reen **C**ity **R**eal **S**tate consta de dos proyectos:
- GCRS.Maps
  - Proyecto donde se desarrollará todo el trabajo con mapas.
  - Servicio CubaMapHandler: servidor de mapas usando SharpMap como framework y datos de OpenStreetMap.
  - Servicio RouteService: permite calcular rutas entre coordenadas del mapa, usando la librería Itinero y datos de OpenStreetMap.
- GCRS.Web
  - Proyecto Asp.Net Mvc 5 donde se desarrollará la aplicación visual, la lógica de la aplicación estará en otros proyectos para garantizar la separación de aspectos.
  - El controlador MapController procesa la interacción del usuario con el visor del mapa.
  - Para la visualización del mapa en el navegador, se usa la librería OpenLayers.
  - Para la visualización de charts, se usa la librería Chart.js.
  - Están añadidos también Bootstrap y jQuery.
  - Recomendamos usar TypeScript para todo el desarrollo, pero no es obligatorio; se usa Webpack para generar un solo fichero compilado.

```
Todas las librerías JavaScripts se han instalado usando Node 6+, 
sus versiones se pueden ver en el fichero package.json.
```

# Extensiones Visual Studio
Las siguientes extensiones son útiles para desarrollar:
- NPM Task Runner: Permite ejecutar las tareas definidas en la sección **scripts** del package.json, solo es necesario si usan TypeScript. Para ver la ventana de ejecutar tareas, ir a View -> Other Windows - Task Runner Explorer.
- Markdown Editor: Visualización del formato Markdown en Visual Studio.

```
Ambas extensiones están en el ftp://casasoft.matcom.uh.cu
```

# Build, para quienes usen TypeScript
Visual Studio contiene un compilador de TypeScript, que convierte cada fichero .ts en un .js con el código JavaScript. Webpack es usado para generar un solo fichero, y así evitar que se tengan que importar en cada vista los scripts necesarios.

Para generar el fichero, se puede ejecutar la tarea **build-dev** desde la ventana de tareas o ejecutar en una consola en el directorio donde está el package.json:
```
npm run build-dev
```	

La tarea **start** ejecuta la anterior y configura Webpack en modo ***watch*** para que cada cambio en los ficheros TypeScript se reflejen en el fichero generado. Los que instalen la extensión NPM Task Runner verán que esta tarea se ejecuta al abrir el proyecto en Visual Studio.

```
npm run start
```	

# Desarrollo en TypeScript
El código se encuentra en la carpeta **src**. El fichero de entrada a la aplicación es **app.ts**. Aquí se configuran las rutas, muy parecido a como se hace en Asp.Net Mvc; esto permite a la aplicación saber cual fichero .ts es responsable de ejecutarse dependiendo de la url.

En la carpeta **shared** se encuentran dos componentes que utilizarán en el desarrollo de la aplicación: **chart-viewer.ts** y **map-viewer.ts**. El primero es usado para generar charts, un uso de ejemplo lo pueden observar en **home/about.ts**; el segundo es el visualizador del mapa, su uso lo pueden observar en **home/map-viewer.ts**

Las interfaces globales se encuentran en **typings.d.ts**.

Para usar un estilo similar de código TypeScript, se usa un ***linter*** con reglas por defecto que mostrará advertencias en la consola o la ventana de tareas.