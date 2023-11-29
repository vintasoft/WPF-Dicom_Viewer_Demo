# VintaSoft WPF DICOM Viewer Demo

This C# project uses <a href="https://www.vintasoft.com/vsimaging-dotnet-index.html">VintaSoft Imaging .NET SDK</a> and demonstrates how to view and annotate DICOM images in WPF:
* Preview single- and multipage DICOM files.
* Display DICOM image with necessary VOI LUT (value of interest lookup table).
* Display DICOM image with/without overlays.
* Animate DICOM images.
* View metadata of DICOM file.
* Annotate DICOM images.


## Screenshot
<img src="vintasoft-wpf-dicom-viewer-demo.png" title="VintaSoft WPF DICOM Viewer Demo">


## Usage
1. Get the 30 day free evaluation license for <a href="https://www.vintasoft.com/vsimaging-dotnet-index.html" target="_blank">VintaSoft Imaging .NET SDK</a> as described here: <a href="https://www.vintasoft.com/docs/vsimaging-dotnet/Licensing-Evaluation.html" target="_blank">https://www.vintasoft.com/docs/vsimaging-dotnet/Licensing-Evaluation.html</a>

2. Update the evaluation license in "CSharp\MainWindow.xaml.cs" file:
   ```
   Vintasoft.Imaging.ImagingGlobalSettings.Register("REG_USER", "REG_EMAIL", "EXPIRATION_DATE", "REG_CODE");
   ```

3. Build the project ("WpfDicomViewerDemo.Net8.csproj" file) in Visual Studio or using .NET CLI:
   ```
   dotnet build WpfDicomViewerDemo.Net8.csproj
   ```

4. Run compiled application and try to view and annotate DICOM images.


## Documentation
VintaSoft Imaging .NET SDK on-line User Guide and API Reference for .NET developer is available here: https://www.vintasoft.com/docs/vsimaging-dotnet/


## Support
Please visit our <a href="https://myaccount.vintasoft.com/">online support center</a> if you have any question or problem.
