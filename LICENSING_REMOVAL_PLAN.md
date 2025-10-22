# Project Sweeper - Licensing Code Removal Plan

## Executive Summary

This document outlines a comprehensive plan for removing all authentication, registration, and licensing code from Project Sweeper. The licensing system was hastily disabled when the software transitioned from commercial to freeware, leaving significant technical debt in the form of commented-out code, conditional compilation directives, and unused dependencies.

## Goals

1. **Complete removal** of all licensing, authentication, and registration code
2. **Simplify build configuration** by eliminating MULTILICENSE preprocessor directives
3. **Reduce external dependencies** by removing dependency on pkhCommon licensing utilities
4. **Improve code maintainability** by eliminating dead code
5. **Clean up user settings** that stored licensing information
6. **Remove security artifacts** (RSA keys, authentication endpoints)

## Impact Assessment

### Files to Modify: 15
### Files to Delete: 5
### Build Configurations to Simplify: 4
### External Dependencies to Review: 1 (pkhCommon)

## Detailed Removal Plan

---

## Phase 1: File Deletions and Major Cleanups

### 1.1 DELETE: ExternalApplication - Licensing.cs

**File:** `/home/user/Project-Sweeper/Project Sweeper/ExternalApplication - Licensing.cs`

**Rationale:** This entire file (15,943 bytes) contains only licensing code:
- Lines 9-115: Commented-out Infralution licensing system
- Lines 117-124: Stub UserIsEntitled() that always returns true
- Lines 126-276: Commented-out authentication logic

**Action:**
```bash
DELETE FILE: ExternalApplication - Licensing.cs
```

**Replacement:** Remove all calls to `ProjectSweeper.UserIsEntitled()`

---

### 1.2 MODIFY: Remove Entitlement Checks from Command Files

#### File 1: LineStyleCleaner/ExtCommands.cs

**Location:** Line 32
**Current Code:**
```csharp
#if !DEBUG
if (!PKHL.ProjectSweeper.ProjectSweeper.UserIsEntitled(commandData))
    return Result.Failed;
#endif
```

**Action:** DELETE lines 30-33 (entire conditional block)

---

#### File 2: LinePatternCleaner/ExtCommands.cs

**Location:** Line 27
**Current Code:**
```csharp
#if !DEBUG
if (!PKHL.ProjectSweeper.ProjectSweeper.UserIsEntitled(commandData))
    return Result.Failed;
#endif
```

**Action:** DELETE lines 25-28 (entire conditional block)

---

#### File 3: TextStyleCleaner/ExtCommands.cs

**Location:** Line 29
**Current Code:**
```csharp
#if !DEBUG
if (!PKHL.ProjectSweeper.ProjectSweeper.UserIsEntitled(commandData))
    return Result.Failed;
#endif
```

**Action:** DELETE lines 27-30 (entire conditional block)

---

#### File 4: FillRegionTypeCleaner/ExtCommand.cs

**Location:** Line 26
**Current Code:**
```csharp
#if !DEBUG
if (!PKHL.ProjectSweeper.ProjectSweeper.UserIsEntitled(commandData))
    return Result.Failed;
#endif
```

**Action:** DELETE lines 24-27 (entire conditional block)

---

### 1.3 MODIFY: ExternalApplication.cs - Remove MULTILICENSE Conditionals

**File:** `/home/user/Project-Sweeper/Project Sweeper/ExternalApplication.cs`

#### Change 1: Remove EULA Button (Lines 109-119)
**Action:** DELETE entire conditional block:
```csharp
#if MULTILICENSE
// EULA button creation code
#endif
```

#### Change 2: Remove Test Entitlement Button (Lines 121-128)
**Action:** DELETE entire conditional block:
```csharp
#if DEBUG
// Test entitlement button
#endif
```

**Expected Result:** Ribbon panel will contain only 5 cleaner tools + Help + About (7 buttons total, down from 9)

---

### 1.4 MODIFY: Help and About ExtCommands.cs

**File:** `/home/user/Project-Sweeper/Project Sweeper/Help and About ExtCommands.cs`

#### Change 1: Clean up AboutBoxCommand (Lines 11-69)

**Current Code:**
```csharp
public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
{
    //#if MULTILICENSE
    //    ... commented code ...
    //#else
    AboutBox aboutbox = new AboutBox();
    aboutbox.ShowDialog();
    return Result.Succeeded;
    //#endif
}
```

**Action:**
- DELETE lines 20-35 (commented-out multilicense code)
- Keep only the active code block (lines 37-39)

#### Change 2: DELETE ApplicationEula Command (Lines 83-95)

**Action:** DELETE entire class:
```csharp
#if MULTILICENSE
public class ApplicationEula : IExternalCommand
{
    // EULA command implementation
}
#endif
```

#### Change 3: DELETE test_Entitlement Command (Lines 98-112)

**Action:** DELETE entire class:
```csharp
#if DEBUG
public class test_Entitlement : IExternalCommand
{
    // Test entitlement command
}
#endif
```

**Result:** File will contain only AboutBoxCommand, ApplicationHelp, and AlwaysAvailableCheck

---

### 1.5 MODIFY: Windows/About Box.xaml.cs

**File:** `/home/user/Project-Sweeper/Project Sweeper/Windows/About Box.xaml.cs`

#### Change 1: Remove Commented Constructors (Lines 38-87)

**Action:** DELETE lines 43-87:
```csharp
//public AboutBox()
//{ ... unlicensed constructor ... }

//public AboutBox(Infralution.Licensing.AuthenticatedLicense license)
//{ ... licensed constructor ... }
```

#### Change 2: Remove Version Check Background Worker (Lines 217-220)

**Current Code:**
```csharp
//BackgroundWorker worker = new BackgroundWorker();
//worker.DoWork += worker_DoWork;
//worker.RunWorkerCompleted += worker_RunWorkerCompleted;
//worker.RunWorkerAsync();
```

**Action:** DELETE lines 217-220

**Result:** About box will only display static product information

---

## Phase 2: Configuration and Settings Cleanup

### 2.1 MODIFY: Constants.cs

**File:** `/home/user/Project-Sweeper/Project Sweeper/Constants.cs`

**Current Code (Lines 11-17):**
```csharp
#if MULTILICENSE
    public static string PRODUCTID = @"82cd28c4-2c4a-41d0-87cf-99cbf0faa369";
    public static string GROUP_NAME = "Project Sweeper ML";
#else
    public static string GROUP_NAME = "Project Sweeper";
    public static string APP_STORE_ID = @"appstore.exchange.autodesk.com:projectsweeper";
#endif
```

**Action:**
- DELETE entire conditional block
- REPLACE with:
```csharp
public static string GROUP_NAME = "Project Sweeper";
```
- REMOVE PRODUCTID constant (not used outside licensing)
- REMOVE APP_STORE_ID constant (Autodesk marketplace ID, no longer needed)

---

### 2.2 MODIFY: Properties/Settings.Designer.cs

**File:** `/home/user/Project-Sweeper/Project Sweeper/Properties/Settings.Designer.cs`

#### Change 1: Keep AddinPath Setting (Lines 29-36)
**Action:** NO CHANGE - This is used for help file location

#### Change 2: DELETE EntCheck Setting (Lines 38-48)
**Action:** DELETE property:
```csharp
[global::System.Configuration.UserScopedSettingAttribute()]
[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
[global::System.Configuration.DefaultSettingValueAttribute("2015-05-01")]
public global::System.DateTime EntCheck {
    get { return ((global::System.DateTime)(this["EntCheck"])); }
    set { this["EntCheck"] = value; }
}
```

---

### 2.3 MODIFY: Properties/Settings.settings

**File:** `/home/user/Project-Sweeper/Project Sweeper/Properties/Settings.settings`

**Action:**
- Open in Settings designer
- DELETE "EntCheck" setting
- KEEP "AddinPath" setting

**Alternative:** Manually edit the XML to remove EntCheck profile

---

### 2.4 MODIFY: app.config

**File:** `/home/user/Project-Sweeper/Project Sweeper/app.config`

**Current Code:**
```xml
<userSettings>
    <PKHL.ProjectSweeper.Properties.Settings>
        <setting name="AddinPath" serializeAs="String">
            <value />
        </setting>
        <setting name="EntCheck" serializeAs="String">
            <value>2015-05-01</value>
        </setting>
    </PKHL.ProjectSweeper.Properties.Settings>
</userSettings>
```

**Action:** DELETE EntCheck setting:
```xml
<userSettings>
    <PKHL.ProjectSweeper.Properties.Settings>
        <setting name="AddinPath" serializeAs="String">
            <value />
        </setting>
    </PKHL.ProjectSweeper.Properties.Settings>
</userSettings>
```

---

## Phase 3: Resource and Localization Cleanup

### 3.1 MODIFY: Resources/Language.resx

**File:** `/home/user/Project-Sweeper/Project Sweeper/Resources/Language.resx`

**Resources to DELETE:**

#### Licensing Dialog Messages:
- LIC_DIA001_Title
- LIC_DIA001_MainInst
- LIC_DIA001_MainCont
- LIC_DIA001_Command1
- LIC_DIA001_Command2
- LIC_DIA002_Title
- LIC_DIA002_MainInst
- LIC_DIA002_MainCont
- LIC_DIA003_Title
- LIC_DIA003_MainInst
- LIC_DIA004_MainCont

#### Licensing Error Messages:
- LIC_MSG_MaxLic
- LIC_MSG_NoSrvr
- LIC_MSG_NotFound
- LIC_MSG_NoUserDat
- LIC_MSG_Revoked

#### EULA Resources:
- EULA_Title
- EULA_IconTip

**Action:** Open Language.resx in resource editor and delete all items starting with "LIC_" or "EULA_"

---

### 3.2 MODIFY: Resources/Language.fr.resx

**File:** `/home/user/Project-Sweeper/Project Sweeper/Resources/Language.fr.resx`

**Action:** Same deletions as Language.resx (French translations of licensing strings)

---

### 3.3 MODIFY: Resources/Language.Designer.cs

**File:** `/home/user/Project-Sweeper/Project Sweeper/Resources/Language.Designer.cs`

**Action:** This file is auto-generated. After modifying .resx files:
1. Right-click Language.resx in Solution Explorer
2. Select "Run Custom Tool" to regenerate Language.Designer.cs
3. Verify licensing properties are removed

---

## Phase 4: Build Configuration Simplification

### 4.1 MODIFY: Project Sweeper.csproj

**File:** `/home/user/Project-Sweeper/Project Sweeper/Project Sweeper.csproj`

#### Change 1: Remove MULTILICENSE from DefineConstants

**Find all instances:**
```xml
<DefineConstants>TRACE;DEBUG;MULTILICENSE</DefineConstants>
<DefineConstants>TRACE;MULTILICENSE</DefineConstants>
```

**Replace with:**
```xml
<DefineConstants>TRACE;DEBUG</DefineConstants>
<DefineConstants>TRACE</DefineConstants>
```

#### Change 2: Simplify Build Configurations (Optional)

**Current configurations:**
- Debug|x64
- Release|x64
- MLTI_Debug|x64
- MLTI_Release|x64

**Recommendation:**
- KEEP: Debug|x64, Release|x64
- DELETE: MLTI_Debug|x64, MLTI_Release|x64

**Action:**
1. Open Configuration Manager in Visual Studio
2. Delete MLTI_Debug configuration
3. Delete MLTI_Release configuration

---

### 4.2 DELETE: Multi-License Add-in Manifests

#### File 1: Project Sweeper - ML - debug.addin
**Action:** DELETE FILE

#### File 2: Project Sweeper - ML - Locked.addin
**Action:** DELETE FILE

**Rationale:** These manifests are for multi-license builds which will be eliminated

**Keep:**
- Project Sweeper - debug.addin (for debug builds)
- Project Sweeper - secure.addin (for release builds)

---

## Phase 5: External Dependency Review

### 5.1 REVIEW: pkhCommon Dependency

**Current Usage Analysis Required:**

#### Known Licensing-Related Usage:
- `pkhCommon.EntitlementHelper.Entitlement()` - Autodesk 360 entitlement checking
- Licensing utilities (type unknown)

#### Known Non-Licensing Usage:
- `pkhCommon.StringHelper` - String manipulation utilities
- Email functionality - Error reporting

**Action Plan:**
1. Search entire codebase for `pkhCommon` references
2. Categorize usage into:
   - Licensing-related (REMOVE)
   - Utility functions (KEEP or REPLACE)
3. If only utility functions remain, assess:
   - Can we inline the utility code?
   - Is the external dependency justified?
   - Should we replace with standard .NET methods?

**Commands to Execute:**
```bash
# Find all pkhCommon references
grep -r "pkhCommon" --include="*.cs"

# Find specific EntitlementHelper usage
grep -r "EntitlementHelper" --include="*.cs"

# Find StringHelper usage
grep -r "StringHelper" --include="*.cs"
```

---

## Phase 6: Verification and Testing

### 6.1 Compilation Verification

**Steps:**
1. Clean solution
2. Rebuild in Debug|x64 configuration
3. Verify no compilation errors
4. Rebuild in Release|x64 configuration
5. Verify no compilation errors

**Expected Warnings to Investigate:**
- Unused using directives
- Unreferenced assemblies
- Obsolete API usage

---

### 6.2 Functional Testing Checklist

Test in Revit 2021:

#### Test 1: Ribbon Panel Loading
- [ ] Revit starts without errors
- [ ] Project Sweeper panel appears in ribbon
- [ ] Panel contains exactly 7 buttons (5 cleaners + Help + About)
- [ ] No EULA or Test Entitlement buttons present

#### Test 2: Command Availability
- [ ] All commands unavailable when no document open
- [ ] All commands available when document open
- [ ] No licensing dialogs appear

#### Test 3: Each Cleaning Module
- [ ] Line Style Cleaner opens and functions
- [ ] Line Pattern Cleaner opens and functions
- [ ] Text Style Cleaner opens and functions
- [ ] Fill Pattern Cleaner opens and functions
- [ ] Fill Region Type Cleaner opens and functions

#### Test 4: Help and About
- [ ] Help button opens help documentation
- [ ] About box displays correct information
- [ ] About box shows no license information

#### Test 5: Logging
- [ ] Log files created in %LOCALAPPDATA%\pkhlineworks\ProjectSweeper\2021\
- [ ] No licensing-related log entries
- [ ] Normal operation logged correctly

---

### 6.3 Code Review Checklist

- [ ] No references to `UserIsEntitled` remain
- [ ] No `#if MULTILICENSE` directives remain
- [ ] No `#if !DEBUG` wrapping entitlement checks remain
- [ ] No commented-out licensing code remains
- [ ] All licensing resource strings removed
- [ ] EntCheck setting removed from all configuration files
- [ ] Multi-license build configurations removed
- [ ] Multi-license .addin manifests deleted
- [ ] ExternalApplication - Licensing.cs deleted
- [ ] pkhCommon usage assessed and cleaned

---

## Phase 7: Documentation Updates

### 7.1 UPDATE: README.md

**Add section:**
```markdown
## License

Project Sweeper is freeware software. Use at your own risk.

Originally developed by pkh Lineworks as commercial software,
this project has been released as freeware for the Revit community.

## History

This is the freeware release with all licensing and registration
code removed. The software is provided as-is without warranty.
```

---

### 7.2 UPDATE: About Box Display

**File:** Windows/About Box.xaml

**Verify text includes:**
- Product name: "Project Sweeper"
- Version: 2021.1.1.0
- Status: "Freeware" or "Free Software"
- No licensing information

---

## Implementation Priority

### Critical Path (Must Complete First):
1. Delete ExternalApplication - Licensing.cs
2. Remove entitlement checks from 4 command files
3. Clean up ExternalApplication.cs (remove MULTILICENSE blocks)
4. Rebuild and test basic functionality

### Secondary Tasks (Complete Next):
5. Clean up Help and About ExtCommands.cs
6. Clean up About Box.xaml.cs
7. Remove EntCheck from settings and configuration
8. Test all modules

### Tertiary Tasks (Cleanup):
9. Remove licensing resource strings
10. Simplify build configurations
11. Delete multi-license .addin files
12. Review pkhCommon dependency

### Final Tasks (Polish):
13. Update documentation
14. Final compilation and testing
15. Create release build

---

## Risk Assessment

### Low Risk:
- Deleting ExternalApplication - Licensing.cs (not referenced after removing entitlement calls)
- Removing entitlement checks (currently bypassed anyway)
- Deleting commented-out code (not compiled)
- Removing resource strings (will show [Missing Resource] if referenced, easy to find)
- Deleting multi-license .addin files (not used)

### Medium Risk:
- Modifying Constants.cs (ensure PRODUCTID not used elsewhere)
- Removing EntCheck setting (could cause errors if accessed)
- Modifying build configurations (backup project first)

### High Risk:
- Modifying pkhCommon usage (need full analysis first)

---

## Rollback Plan

**Before starting:**
1. Create git branch: `feature/remove-licensing-code`
2. Commit current state
3. Work in isolated branch

**If issues occur:**
```bash
git checkout main
git branch -D feature/remove-licensing-code
```

**After successful completion:**
```bash
git checkout main
git merge feature/remove-licensing-code
git tag "v2021.1.1.1-freeware"
```

---

## Success Criteria

### Code Quality:
- [ ] Zero compilation errors
- [ ] Zero compilation warnings related to removed code
- [ ] All preprocessor directives related to licensing removed
- [ ] No commented-out code blocks
- [ ] No unused dependencies

### Functionality:
- [ ] All 5 cleaning modules operational
- [ ] No licensing dialogs or checks
- [ ] Help and About work correctly
- [ ] Logging works correctly
- [ ] No runtime errors

### Maintainability:
- [ ] Simplified build configuration (2 configs instead of 4)
- [ ] Reduced external dependencies
- [ ] Clear, uncommented code
- [ ] Updated documentation

---

## Estimated Effort

| Phase | Estimated Time |
|-------|----------------|
| Phase 1: File Deletions | 2 hours |
| Phase 2: Configuration Cleanup | 1 hour |
| Phase 3: Resource Cleanup | 1 hour |
| Phase 4: Build Configuration | 1 hour |
| Phase 5: Dependency Review | 2-4 hours |
| Phase 6: Testing | 2 hours |
| Phase 7: Documentation | 1 hour |
| **Total** | **10-12 hours** |

---

## Files Inventory

### Files to DELETE (5):
1. Project Sweeper/ExternalApplication - Licensing.cs
2. Project Sweeper - ML - debug.addin
3. Project Sweeper - ML - Locked.addin
4. [Generated] bin/*/Project Sweeper ML.dll (build artifacts)
5. [Optional] MLTI_* build configuration folders

### Files to MODIFY (15):
1. Project Sweeper/ExternalApplication.cs
2. Project Sweeper/LineStyleCleaner/ExtCommands.cs
3. Project Sweeper/LinePatternCleaner/ExtCommands.cs
4. Project Sweeper/TextStyleCleaner/ExtCommands.cs
5. Project Sweeper/FillRegionTypeCleaner/ExtCommand.cs
6. Project Sweeper/Help and About ExtCommands.cs
7. Project Sweeper/Windows/About Box.xaml.cs
8. Project Sweeper/Constants.cs
9. Project Sweeper/Properties/Settings.Designer.cs
10. Project Sweeper/Properties/Settings.settings
11. Project Sweeper/app.config
12. Project Sweeper/Resources/Language.resx
13. Project Sweeper/Resources/Language.fr.resx
14. Project Sweeper/Resources/Language.Designer.cs (auto-regenerated)
15. Project Sweeper/Project Sweeper.csproj

### Files to REVIEW (uncertain count):
- All files with pkhCommon references (to be determined)

---

## Post-Cleanup Benefits

1. **Reduced code complexity** - ~500-1000 lines of dead code removed
2. **Simplified build process** - 2 configurations instead of 4
3. **Improved maintainability** - No conditional compilation for licensing
4. **Reduced dependencies** - Potentially remove pkhCommon entirely
5. **Cleaner codebase** - No commented-out blocks
6. **Clear intent** - Obvious this is freeware software
7. **Easier onboarding** - New developers don't need to understand licensing history

---

## Conclusion

This plan provides a systematic approach to removing all licensing, authentication, and registration code from Project Sweeper. The work is straightforward with low risk, as most licensing code is already disabled. The primary challenge will be reviewing the pkhCommon dependency to determine if it can be eliminated entirely.

Following this plan will result in a clean, maintainable codebase that clearly reflects the software's freeware status.
