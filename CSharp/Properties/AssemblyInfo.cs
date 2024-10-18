﻿using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

#if REMOVE_DICOM_PLUGIN
#error DicomViewerDemo project cannot be used without VintaSoft DICOM .NET Plug-in.
#endif

[assembly: AssemblyTitle("VintaSoft WPF DICOM Viewer Demo")]
[assembly: AssemblyDescription("Shows how to view and annotate DICOM files.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("VintaSoft LLC")]
[assembly: AssemblyProduct("VintaSoft Imaging .NET SDK")]
[assembly: AssemblyCopyright("Copyright VintaSoft LLC 2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly
)]

[assembly: AssemblyVersion("12.5.7.1")]
[assembly: AssemblyFileVersion("12.5.7.1")]
