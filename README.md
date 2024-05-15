Problem:
Memory Leak issue in Aspose.Pdf.Document.Save method that takes MemoryStream as argument.

Versions:
NET 7.0 ConsoleApp -> Include="Aspose.PDF" Version="24.2.0" (Tried with other versions up to latest 24.5.1)

Reproduce:
Example consoleapp is provided in this repository.
Removing FloatingBox is fixed the issue, this could be a start point to investigate...
