# AutoTagSheetViews

A Revit plugin that automatically adds wall tags with leaders and custom offset logic in a selected viewport.

## Features

- Select a viewport on a sheet
- Automatically tags all walls in the view
- Tags include leaders and are offset perpendicularly to wall orientation
- Supports Revit 2025

## Installation

1. Build the solution in Visual Studio
2. Copy the `.dll` and `.addin` file to: `%APPDATA%\Autodesk\Revit\Addins\2025\`

## Demo

[Demo Video](https://www.loom.com/share/b11d7cc6cf9b429baeddfe31466fb089?sid=dcc0e50c-99c4-4ebd-b88e-96f45c85523a)

## Future Improvements

- UI for selecting category (e.g., doors, windows)
- Option to avoid tagging already-tagged elements
- Smarter offset logic for dense plans

