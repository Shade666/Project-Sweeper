# Project Sweeper - Architecture Review

## Executive Summary

Project Sweeper is a professional-grade Revit plugin that provides cleaning and optimization tools for Revit project files. The codebase consists of approximately 17,782 lines of C# and XAML code, organized into a modular architecture with five independent cleaning tools. The application was originally a commercial product with a sophisticated licensing system, which has been disabled to transition the software to freeware.

## 1. Technology Stack

### Core Framework
- **.NET Framework 4.8** - Target framework
- **C#** - Primary language (2019+ syntax features)
- **Platform:** x64 (AMD64) architecture only
- **Output:** Class Library (DLL)

### Revit Integration
- **RevitAPI 2021** - Core Revit automation API
- **RevitAPIUI 2021** - Revit user interface framework
- **Revit Version:** 2021

### UI Framework
- **WPF (Windows Presentation Foundation)**
  - WindowsBase
  - PresentationCore
  - PresentationFramework
  - System.Xaml
- **Windows Forms** - Limited use for interop scenarios

### Key Dependencies

| Library | Version | Purpose |
|---------|---------|---------|
| log4net | Latest | Comprehensive logging framework |
| Fody | 3.0.0 | Assembly IL weaving |
| Costura.Fody | 2.0.0 | Embed dependent assemblies into output DLL |
| pkhCommon | 1.2.0.0 | Custom utility library (external) |
| WPFLocalizeExtension | Custom | Multi-language support |
| XAMLMarkupExtensions | Custom | XAML markup extensions |

### Build System
- **MSBuild** via Visual Studio project system
- **Build Configurations:**
  - Debug|x64
  - Release|x64
  - MLTI_Debug|x64 (Multi-License Debug)
  - MLTI_Release|x64 (Multi-License Release)
- **Preprocessor Directives:** DEBUG, MULTILICENSE, TRACE

## 2. Project Structure

```
Project-Sweeper/
├── Project Sweeper/                    # Main project folder
│   ├── LineStyleCleaner/              # Line style cleaning module
│   │   ├── ExtCommands.cs             # Command implementation (44KB)
│   │   ├── LSC_MainWindow.xaml/cs     # Main UI window
│   │   └── LineStyleDefinition.cs     # Data model
│   ├── LinePatternCleaner/            # Line pattern cleaning module
│   │   ├── ExtCommands.cs             # Command implementation (16KB)
│   │   ├── LPC_MainWindow.xaml/cs     # Main UI window
│   │   └── LinePatternDefinition.cs   # Data model
│   ├── TextStyleCleaner/              # Text style cleaning module
│   │   ├── ExtCommands.cs             # Command implementation (26KB)
│   │   ├── TSC_MainWindow.xaml/cs     # Main UI window
│   │   └── TextStyleDefinition.cs     # Data model
│   ├── FillPatternCleaner/            # Fill pattern cleaning module
│   │   ├── ExtCommands.cs             # Command implementation (49KB)
│   │   ├── FPC_MainWindow.xaml/cs     # Main UI window
│   │   ├── FillPatternDefinition.cs   # Data model
│   │   └── ProgressBarWindow.xaml/cs  # Progress tracking UI
│   ├── FillRegionTypeCleaner/         # Fill region type cleaning
│   │   ├── ExtCommand.cs              # Command implementation (23KB)
│   │   ├── FRTC_MainWindow.xaml/cs    # Main UI window
│   │   └── FillRegionTypeDefinition.cs # Data model
│   ├── FillPatternViewer/             # Fill pattern visualization
│   │   ├── FillPatternViewerControlWpf.xaml/cs
│   │   ├── BitmapSourceConverter.cs
│   │   └── BitmapToImageSourceConverter.cs
│   ├── LinePatternViewer/             # Line pattern visualization
│   │   └── LinePatternViewerControlWpf.xaml/cs
│   ├── Windows/                       # Shared UI components
│   │   ├── About Box.xaml/cs          # About dialog
│   │   ├── Input Box.xaml/cs          # Generic input dialog
│   │   ├── ListViewsWindow.xaml/cs    # View list display
│   │   ├── ListUsersWindow.xaml/cs    # User list display
│   │   ├── ResultWindow.xaml/cs       # Results summary
│   │   └── TemplateWindow.xaml/cs     # Template management
│   ├── Resources/                     # Application resources
│   │   ├── Language.resx              # English strings
│   │   ├── Language.fr.resx           # French strings
│   │   └── *.png                      # Ribbon icons (16x16, 32x32)
│   ├── Properties/                    # Project properties
│   │   ├── AssemblyInfo.cs
│   │   ├── Settings.settings
│   │   └── Settings.Designer.cs
│   ├── ExternalApplication.cs         # Main entry point
│   ├── ExternalApplication - Licensing.cs  # Licensing code (DISABLED)
│   ├── Help and About ExtCommands.cs  # Help/About commands
│   ├── AssetDefinition.cs             # Asset data model
│   ├── BaseStyleDefinition.cs         # Base style abstraction
│   ├── ViewOwnerDefinition.cs         # View-owned style abstraction
│   ├── Constants.cs                   # Application constants
│   ├── LocalizationProvider.cs        # Localization utilities
│   ├── TemplateFile.xsd              # Template schema
│   ├── TemplateFile.xml              # Template configuration
│   ├── TemplateFile.cs               # Auto-generated from XSD
│   ├── app.config                     # Application configuration
│   ├── projectsweeper.log4net.config # Logging configuration
│   ├── FodyWeavers.xml               # Fody weaving settings
│   └── *.addin                        # Revit add-in manifest files
├── Project Sweeper 2021.sln           # Visual Studio solution
├── README.md                          # Project readme
└── html_help_files.zip               # Help documentation

Total: 45 C# files, 21 XAML files (~17,782 lines of code)
```

## 3. Architecture Overview

### 3.1 Architectural Pattern

Project Sweeper follows a **modular plugin architecture** with the following characteristics:

1. **Revit External Application Pattern** - Implements `IExternalApplication` for lifecycle management
2. **Command Pattern** - Each cleaning tool implements `IExternalCommand`
3. **MVVM (Model-View-ViewModel)** - For WPF UI components
4. **Template Method Pattern** - Abstract base classes define common behavior
5. **Factory Pattern** - Definition classes constructed from Revit elements

### 3.2 Core Architecture Components

```
┌─────────────────────────────────────────────────────────────┐
│                        Revit API                             │
│                  (IExternalApplication)                      │
└───────────────────────┬─────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────────────────┐
│              ExternalApplication.cs                          │
│  - OnStartup(): Initialize plugin, create ribbon            │
│  - OnShutdown(): Cleanup                                     │
└───────────────┬─────────────────────────────────────────────┘
                │
                ├──► Ribbon Panel Creation (7 buttons)
                │
                ▼
┌─────────────────────────────────────────────────────────────┐
│                  External Commands                           │
│              (IExternalCommand)                              │
├─────────────────────────────────────────────────────────────┤
│  ┌───────────────┐  ┌────────────────┐  ┌──────────────┐  │
│  │ LineStyle     │  │ LinePattern    │  │ TextStyle    │  │
│  │ Cleaner       │  │ Cleaner        │  │ Cleaner      │  │
│  └───────────────┘  └────────────────┘  └──────────────┘  │
│  ┌───────────────┐  ┌────────────────┐                     │
│  │ FillPattern   │  │ FillRegionType │                     │
│  │ Cleaner       │  │ Cleaner        │                     │
│  └───────────────┘  └────────────────┘                     │
└───────────────┬─────────────────────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────────────────────┐
│                 Definition Layer                             │
│           (BaseStyleDefinition hierarchy)                    │
├─────────────────────────────────────────────────────────────┤
│                 BaseStyleDefinition                          │
│                 (Abstract Base)                              │
│                         │                                    │
│         ┌───────────────┴────────────────┐                  │
│         ▼                                 ▼                  │
│  ViewOwnerDefinition          FillPatternDefinition          │
│  (Abstract)                                                  │
│         │                                                    │
│    ┌────┴─────┬──────────┬────────────┐                    │
│    ▼          ▼          ▼            ▼                     │
│  LineStyle  TextStyle  LinePattern  FillRegionType          │
│  Definition Definition Definition   Definition              │
└─────────────────────────────────────────────────────────────┘
                │
                ▼
┌─────────────────────────────────────────────────────────────┐
│                    WPF UI Layer                              │
│  - XAML Views with data binding                             │
│  - ObservableCollection<T> for reactive updates            │
│  - INotifyPropertyChanged for property changes             │
│  - Shared windows (About, Input, ListView, Result)         │
└─────────────────────────────────────────────────────────────┘
```

### 3.3 Class Hierarchy

**BaseStyleDefinition** (Abstract)
- Properties: StyleName, ItsId, NumberOfUses, StyleColour, IsDeleteable
- Methods: StyleToBeDeleted, NewStyle (conversion target)
- Implements: INotifyPropertyChanged

**ViewOwnerDefinition** (Abstract, extends BaseStyleDefinition)
- Additional Properties: OwnerViews, OwnerSchedules
- Used by styles that belong to specific views

**Concrete Implementations:**
1. LineStyleDefinition (extends ViewOwnerDefinition)
2. TextStyleDefinition (extends ViewOwnerDefinition)
3. LinePatternDefinition (extends ViewOwnerDefinition)
4. FillRegionTypeDefinition (extends ViewOwnerDefinition)
5. FillPatternDefinition (extends BaseStyleDefinition)

**AssetDefinition**
- Represents elements that use styles/patterns
- Types: Material, Component, Region, Family
- Constructors for: GraphicsStyle, Material, Element, FilledRegionType, Family

## 4. Module Descriptions

### 4.1 Line Style Cleaner (LSC)
**Purpose:** Remove unused line styles from Revit projects

**Capabilities:**
- Scans all line categories (Thin, Medium, Wide, Generic, Demolished, Hidden, Overhead, Beyond, Center, Hidden Lines)
- Differentiates between model lines and detail lines
- Tracks which views use each style
- Allows conversion to alternative styles before deletion
- Protects system categories (negative CategoryId)

**Key Files:**
- LineStyleCleaner/ExtCommands.cs:32 - Main command with entitlement check
- LineStyleCleaner/LSC_MainWindow.xaml.cs - UI implementation
- LineStyleDefinition.cs - Data model

### 4.2 Line Pattern Cleaner (LPC)
**Purpose:** Remove unused line patterns

**Capabilities:**
- Collects all LinePatternElement objects
- Shows pattern properties and usage
- Supports conversion to solid pattern
- Visual preview of patterns

**Key Files:**
- LinePatternCleaner/ExtCommands.cs:27 - Main command
- LinePatternCleaner/LPC_MainWindow.xaml.cs - UI
- LinePatternDefinition.cs - Data model
- LinePatternViewer/ - Pattern visualization

### 4.3 Text Style Cleaner (TSC)
**Purpose:** Remove unused text/annotation styles

**Capabilities:**
- Manages TextNoteType elements
- Properties: font, size, bold, italic, underline
- Graphic settings: color, weight, background, border
- Tracks usage in views and schedules
- Style comparison window

**Key Files:**
- TextStyleCleaner/ExtCommands.cs:29 - Main command
- TextStyleCleaner/TSC_MainWindow.xaml.cs - UI
- TextStyleDefinition.cs - Data model

### 4.4 Fill Pattern Cleaner (FPC)
**Purpose:** Remove unused fill patterns from entire project

**Capabilities:**
- Scans materials, families, components for pattern usage
- Multi-threaded family scanning with progress bar
- Handles both model and drafting patterns
- Supports pattern conversion
- Manages solid fills separately
- Analyzes fill pattern grid properties

**Key Files:**
- FillPatternCleaner/ExtCommands.cs - Main command (49KB, largest module)
- FillPatternCleaner/FPC_MainWindow.xaml.cs - UI
- FillPatternCleaner/ProgressBarWindow.xaml.cs - Progress tracking
- FillPatternDefinition.cs - Data model
- FillPatternViewer/ - Pattern rendering

### 4.5 Fill Region Type Cleaner (FRTC)
**Purpose:** Manage filled region types

**Capabilities:**
- Ensures at least one region type always exists
- Foreground and background pattern management
- Masking region support
- Type conversion

**Key Files:**
- FillRegionTypeCleaner/ExtCommand.cs:26 - Main command
- FillRegionTypeCleaner/FRTC_MainWindow.xaml.cs - UI
- FillRegionTypeDefinition.cs - Data model

## 5. Integration Points

### 5.1 Revit Entry Point

**File:** ExternalApplication.cs

**OnStartup Method:**
- Configures log4net logging
- Sets AddinPath user setting
- Creates "Project Sweeper" ribbon panel
- Adds 7 pushbuttons with icons
- Loads contextual help system
- Conditionally includes EULA button (#if MULTILICENSE)

**Ribbon Panel Structure:**
```
Project Sweeper Panel
├── Line Style Cleaner (LSC)
├── Line Pattern Cleaner (LPC)
├── Text Style Cleaner (TSC)
├── Fill Pattern Cleaner (FPC)
├── Fill Region Type Cleaner (FRTC)
├── Help
└── About
[Optional: EULA - if MULTILICENSE defined]
```

### 5.2 Command Availability

Each command implements `IExternalCommandAvailability`:

```csharp
public class command_AvailableCheck : IExternalCommandAvailability {
    public bool IsCommandAvailable(UIApplication uiapp, CategorySet tempCatSet)
    {
        if (uiapp.ActiveUIDocument == null) return false;
        #if !DEBUG
        return PKHL.ProjectSweeper.ProjectSweeper.UserIsEntitled(uiapp);
        #else
        return true;
        #endif
    }
}
```

**Availability Logic:**
- Disabled when no active document
- Entitlement check (currently bypassed - always returns true)
- DEBUG builds skip entitlement

### 5.3 Add-in Manifest Files

Four manifest variants:
1. **Project Sweeper - debug.addin** - Standard debug build
2. **Project Sweeper - secure.addin** - Release build
3. **Project Sweeper - ML - debug.addin** - Multi-license debug
4. **Project Sweeper - ML - Locked.addin** - Multi-license release

**Typical Structure:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<RevitAddIns>
  <AddIn Type="Application">
    <Name>Project Sweeper</Name>
    <Assembly>Project Sweeper.dll</Assembly>
    <FullClassName>PKHL.ProjectSweeper.ProjectSweeper</FullClassName>
    <ClientId>...</ClientId>
    <VendorId>PKHL</VendorId>
    <VendorDescription>pkh Lineworks</VendorDescription>
  </AddIn>
</RevitAddIns>
```

## 6. Data Flow

### Typical Cleaning Operation Flow:

1. **User clicks ribbon button** → Revit calls Execute() on IExternalCommand
2. **Entitlement check** → UserIsEntitled() (currently always returns true)
3. **Data collection** → Query Revit document for styles/patterns
4. **Definition creation** → Build Definition objects (e.g., LineStyleDefinition)
5. **Usage analysis** → Find all elements using each style/pattern
6. **UI display** → Show ObservableCollection in WPF DataGrid
7. **User interaction** → Select items to delete or convert
8. **Validation** → Ensure deletions won't break model
9. **Transaction** → Execute deletion/conversion in Revit transaction
10. **Results** → Display summary window with operation results

### Example: Line Style Cleaner Flow

```
User clicks LSC button
    ↓
Execute() called → EntitleCheck (passes)
    ↓
FilteredElementCollector → Get all Categories
    ↓
Filter: OST_Lines category, negative CategoryId
    ↓
Get GraphicsStyle from each Category
    ↓
Create LineStyleDefinition for each style
    ↓
Scan document for usage of each style
    ↓
Populate OwnerViews dictionary
    ↓
Display in LSC_MainWindow (WPF DataGrid)
    ↓
User selects styles to delete/convert
    ↓
Transaction: Delete or ChangeTypeId
    ↓
Show ResultWindow with summary
```

## 7. Configuration and Settings

### 7.1 Application Configuration (app.config)

```xml
<userSettings>
    <AddinPath>         <!-- Runtime set to DLL location -->
    <EntCheck>          <!-- License check timestamp -->
</userSettings>
```

### 7.2 Logging Configuration (projectsweeper.log4net.config)

**Log Locations:**
```
%LOCALAPPDATA%\pkhlineworks\ProjectSweeper\2021\
├── logfile.txt      (Rolling, 1MB max, 5 backups)
└── errorfile.txt    (Error level only)
```

**Appenders:**
- RollingFileAppender (general logging)
- ErrorFileAppender (errors only)

### 7.3 User Settings

**Stored in:** Windows user settings (registry)

**Settings:**
1. **AddinPath** (string)
   - Path to Project Sweeper.dll
   - Set during OnStartup()

2. **EntCheck** (DateTime)
   - Last entitlement check timestamp
   - Default: 2015-05-01
   - Used for 7-day caching of entitlement verification

## 8. Localization

### Supported Languages:
- English (default): Language.resx
- French: Language.fr.resx

### Localization System:
- WPFLocalizeExtension library
- LocalizationProvider class wraps LocExtension
- XAML markup extensions for runtime language switching

### Resource Categories:
- UI labels and buttons
- Dialog messages
- Error messages
- Licensing messages (LIC_DIA001-004, LIC_MSG_*)
- Help tooltips

## 9. Help System

### Contextual Help:
- HTML help files in html_help_files.zip
- ContextualHelp class provides F1 integration
- Help files for each cleaning module

### Help Files:
- LineStyleCleaner.htm
- LinePatternCleaner.htm
- TextStyleCleaner.htm
- FillPatternCleaner.htm
- FillRegionTypeCleaner.htm

## 10. Build and Deployment

### Build Process:
1. Compile C# → Project Sweeper.dll
2. Fody weaving → Costura embeds dependencies
3. Copy to output directory
4. Include appropriate .addin manifest

### Deployment:
1. Copy DLL to Revit add-ins folder
2. Copy .addin manifest to same folder
3. Revit loads on next startup

### Debug Configuration:
- StartProgram: Revit.exe
- Allows F5 debugging from Visual Studio

## 11. Dependencies on External Libraries

### Critical External Dependency: pkhCommon (version 1.2.0.0)

**Purpose:** Utility library with shared functionality

**Known Components:**
- StringHelper - String manipulation utilities
- Email functionality - Error reporting
- EntitlementHelper.Entitlement() - License validation (disabled)
- Licensing utilities

**Impact:** Removing licensing code will reduce or eliminate dependency on pkhCommon

### Other Dependencies:
- WPFLocalizeExtension (custom build) - Required for multi-language
- XAMLMarkupExtensions - Required for XAML features
- log4net - Required for logging
- Costura.Fody - Build-time only

## 12. Code Quality and Standards

### Positive Aspects:
- Clear separation of concerns
- Consistent naming conventions
- Comprehensive commenting
- Proper use of design patterns
- XAML-based UI with data binding
- Transaction-based Revit modifications
- Error handling and logging

### Areas for Improvement:
- Large command files (up to 49KB)
- Commented-out code blocks (licensing)
- Multiple build configurations increase complexity
- External dependency on pkhCommon
- No unit tests identified

## 13. Security Considerations

### Current State:
- No active authentication or authorization
- UserIsEntitled() always returns true
- Licensing infrastructure disabled but present

### Embedded Sensitive Data (Inactive):
- RSA encryption keys in ExternalApplication - Licensing.cs
- Authentication server URL: http://www.pkhlineworks.ca/authenticate/AuthenticationService.asmx
- Product GUIDs and App Store IDs

**Recommendation:** Remove all licensing code to eliminate security artifacts

## 14. Performance Considerations

### Optimization Strategies:
- Multi-threaded family scanning (Fill Pattern Cleaner)
- Progress bar for long operations
- Cached entitlement checks (7-day cache)
- FilteredElementCollector for efficient Revit queries

### Potential Bottlenecks:
- Large family scanning operations
- Document-wide style usage analysis
- UI updates during scanning (WPF threading)

## 15. Metadata

| Property | Value |
|----------|-------|
| Solution | Project Sweeper 2021.sln |
| Project GUID | {E4213E54-E339-4502-A0A3-B0B0957E7C2B} |
| Assembly Name | Project Sweeper |
| Namespace | PKHL.ProjectSweeper |
| Version | 2021.1.1.0 |
| Company | pkh Lineworks |
| Copyright | 2020 |
| Target Revit | 2021 |
| .NET Framework | 4.8 |
| Platform | x64 |

## 16. Summary

Project Sweeper is a well-architected Revit plugin demonstrating professional software engineering practices:

**Strengths:**
- Clean modular architecture
- Comprehensive functionality for Revit cleanup tasks
- Professional WPF UI with localization
- Proper use of Revit API patterns
- Extensive logging and error handling

**Technical Debt:**
- Disabled licensing infrastructure (primary cleanup target)
- Dependency on external pkhCommon library
- Multiple build configurations
- No automated testing

**Recommendation:** Proceed with systematic removal of licensing code to fully transition to freeware, reduce external dependencies, and simplify the codebase.
