# Configuración del Proyecto
Este documento proporciona instrucciones sobre cómo configurar y ejecutar el proyecto. Asegúrate de seguir los prerrequisitos antes de comenzar.

## Prerrequisitos
Antes de comenzar, asegúrate de tener instalado lo siguiente:

1) Visual Studio Community 2022: Descarga e instala la última versión de Visual Studio Community desde 
https://visualstudio.microsoft.com/downloads/. Durante la instalación, asegúrate de seleccionar la carga de trabajo  "ASP.NET y desarrollo web".

2) SDK de .NET 6: Descarga e instala el SDK de .NET 6 desde https://dotnet.microsoft.com/en-us/download. Esto proporciona las herramientas
necesarias para construir y ejecutar aplicaciones .NET. NOTA: Este paso puede hacerse en la instalación VIsual Studio Community

3) Administrador de paquetes NuGet: Visual Studio integra NuGet, pero también puedes descargar la herramienta de línea de comandos desde 
https://www.nuget.org/. Esta herramienta ayuda a administrar las dependencias del proyecto.


# Integración de HtmlAgilityPack:

1) Instalación de NuGet: El proyecto utiliza HtmlAgilityPack 1.11.61, que se ha instalado mediante NuGet. Esto garantiza que las bibliotecas 
necesarias sean accesibles dentro del proyecto.

2) Raspado web: El archivo HomeController.cs en la carpeta de controladores aprovecha HtmlAgilityPack para extraer información de productos a partir de respuestas 
HTML obtenidas de Google Shopping y "La Casa del Electrodoméstico".

3) Manipulación del DOM: HtmlAgilityPack proporciona una forma conveniente de analizar y manipular documentos HTML, lo que permite
que la aplicación navegue y extraiga de manera eficiente datos relevantes de las páginas web.






## Ejecución de la aplicación:

1)
Establece el proyecto como proyecto de inicio (si no está ya establecido).
2) Presiona F5 (o haz clic en el botón "Ejecutar sin depuración") para iniciar la aplicación.
3) La aplicación normalmente se ejecutará en tu servidor de desarrollo local (por ejemplo, http://localhost:5000).


## Comprensión del código:

El proyecto consta de dos partes principales: el controlador (HomeController.cs) y la vista (Index.cshtml).

1) HomeController.cs: Este archivo maneja la interacción del usuario a través del formulario de búsqueda. Procesa el término de búsqueda, realiza raspado web utilizando HtmlAgilityPack y devuelve los resultados.

2) Index.cshtml: Este archivo muestra el formulario de búsqueda y actualiza dinámicamente la sección de resultados utilizando AJAX cuando se realiza una búsqueda.

