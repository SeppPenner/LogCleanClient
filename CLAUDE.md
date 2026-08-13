# Project rules for Claude

## What this is

LogCleanClient is a small Windows Forms tool that deletes files from a list of configured folders.
Every folder comes with a file filter, and every file whose name ends with one of the filter entries
is deleted. After a run a modal report dialog lists the searched folders and every deleted file. The
repository is an application, it is **not** published as a NuGet package: no `GeneratePackageOnBuild`,
no push script. It ships as an Inno Setup installer that is committed into the repository.

One solution `src/LogCleanClient.sln` with exactly one project:

- `src/LogCleanClient/LogCleanClient.csproj`, `OutputType` `WinExe`, `UseWindowsForms`,
  `ApplicationIcon` `Clean.ico`, `RuntimeIdentifiers` `win-x64`.

There is no test project, no `.github` folder and no pipeline file in this repository.

Layout inside `src/LogCleanClient`:

- `Program.cs`: `Main` with `[STAThread]`, runs the `Main` form. Nothing else.
- `Main.cs` plus `Main.Designer.cs` and `Main.resx`: the only real logic. The constructor calls
  `InitializeComponent`, `InitializeCaption`, `InitializeLanguageManager`, `LoadLanguagesToCombo` and
  `LoadConfig` in that order, the order matters. `LoadConfig` reads `Config.xml` next to the
  executable, `BackgroundCleanWork` counts and deletes on a `BackgroundWorker`,
  `BackgroundCleanCompleted` fills and shows the report dialog.
- `ReportDialog.cs` plus `ReportDialog.Designer.cs` and `ReportDialog.resx`: a read only
  `RichTextBox` and an ok button. `AddTextToRichTextBox` is the only public member.
- `Config.cs`: the deserialized `Config.xml`, nothing but a `List<LogModel>`. `LogModel.cs`: one
  folder with its `FileFilter`, plus `FileAmount` which is filled at runtime and is not part of the
  file.
- `Config.xml`: the sample configuration, copied to the output directory.
- `languages/de-DE.xml` and `languages/en-US.xml`: six keys each, read by
  `HaemmerElectronics.SeppPenner.Language`.
- `GlobalUsings.cs`: all usings of the project.
- `License.txt` and `Clean.ico`: shipped with the installer, `License.txt` is also the license file
  of the Inno Setup script.

`Setup/` holds `LogCleanClient-Setup.iss` (the Inno Setup script), `build-setup-files.bat` (cleans
`bin` and `obj`, publishes, removes the `*.pdb`) and the built `LogCleanClient-Setup.exe`, which is
tracked.

Repository root: `README.md` (the only user documentation, spelled in upper case here, the sibling
repositories use `Readme.md`), `Changelog.md`, `License.txt` (MIT), `Screenshot_DE.PNG`,
`Screenshot_EN.PNG`, `.gitattributes` and `.gitignore`. There is no `Updating.md` and no
`HowToUse.md`.

## Build

```powershell
dotnet build src/LogCleanClient.sln -c Release
```

```powershell
cd Setup
call .\build-setup-files.bat
```

- Single target framework `net9.0-windows`, no multi-targeting. `RuntimeIdentifiers` is `win-x64`.
- All build properties live directly in `src/LogCleanClient/LogCleanClient.csproj`. There is **no**
  `Directory.Build.props` in this repository.
- `TreatWarningsAsErrors` is enabled, so every warning breaks the build, NuGet warnings (`NU****`)
  from restore included. A clean build reports zero warnings, keep it that way.
- `NU1803` (HTTP source usage during restore) is the one warning suppressed via `NoWarn`. Fix
  warnings instead of extending that list. `NuGetAudit` and `NuGetAuditMode=all` are on, so a
  vulnerable transitive package fails the build too.
- Versions come from GitVersion.MsBuild out of the git tags, for example `1.0.8-1` for the first
  commit after tag `1.0.7`. Never edit a version property or an assembly version by hand.
- Restore needs nuget.org. Several private feeds are configured globally on this machine. If one of
  them answers 404 for public packages, restore fails with `NU1301`. Then build with an explicit
  source:
  `dotnet build src/LogCleanClient.sln --source https://api.nuget.org/v3/index.json`.
- There are no tests. A behaviour change is verified by starting the published executable and by
  running a clean against a throwaway folder, never by claiming it works. Never claim a run
  happened without running it.
- `build-setup-files.bat` does a `cd ..\src` relative to the current directory, so it has to be
  started from the `Setup` folder. In this environment `NoDefaultCurrentDirectoryInExePath` is set,
  which means cmd does not search the current directory, so the call needs the leading `.\`.

## Code conventions

Follow the surrounding code, it is consistent throughout every file:

- File header comment block with `<copyright file="..." company="Hämmer Electronics">` and a
  `<summary>`, then the file-scoped namespace.
- XML doc comments on every type and every member, private members included, no exceptions.
- `Nullable`, `ImplicitUsings` and `LangVersion latest` are enabled. `ILanguage.GetWord` returns
  `string?` and yields `null` for an unknown key, it does not fall back to another language.
- New `using` directives go into `GlobalUsings.cs`, inside the existing `#pragma warning disable
  IDE0065` block, never at the top of a file. The editorconfig requires usings inside the namespace
  (`csharp_using_directive_placement=inside_namespace:warning`), which global usings cannot satisfy,
  that is what the pragma is for. Do not add other pragmas. The comment text in that block is German
  because Visual Studio generated it, leave it alone.
- Fields, properties, methods and events are always accessed with `this.` qualification
  (`dotnet_style_qualification_for_*` at severity `warning`).
- `src/.editorconfig` also enforces braces everywhere, no multiple blank lines, four spaces, CRLF,
  UTF-8, file scoped namespaces, `System` usings sorted first and `IDE0005` as warning. Analyzer
  warnings are fixed, not silenced.
- The C# files are UTF-8 without BOM with CRLF. The XML files (`Config.xml`, both language files)
  are UTF-8 without BOM with CRLF as well, but they are indented with **tabs**, not with spaces.
  Edit them so that tabs, CRLF and the missing BOM survive, and check afterwards. The
  `.editorconfig` `[*]` section claims spaces, the files predate it, do not reformat them.
- `Main.Designer.cs` and `ReportDialog.Designer.cs` are generated by the Windows Forms designer and
  use the old fully qualified `System.Windows.Forms.` style. Leave that style alone.

## Known quirks

Do not silently "clean up" these, they are existing behaviour:

- **The filter matches by `EndsWith`, not by extension.** `FileFilter` is split on `|` and every
  entry is compared with `file.FullName.EndsWith(entry)`. With the documented entries (`.log`,
  `.txt`) that behaves like an extension match, but an entry without the leading dot (`log`) would
  also match `catalog`.
- **An empty filter entry used to delete everything.** `"anything".EndsWith("")` is true, so a
  `FileFilter` of `.log|` or an empty `FileFilter` deleted every file in the folder. Since version
  1.0.8.0 the entries are trimmed, empty ones are dropped and a `LogModel` without a single usable
  entry is skipped instead of cleaned. That is the safe direction, do not turn it back into a
  wildcard.
- **A failed delete aborts the run and is reported.** `File.Delete` on a locked log file throws, the
  `BackgroundWorker` parks that exception in `RunWorkerCompletedEventArgs.Error`. Since version
  1.0.8.0 `BackgroundCleanCompleted` shows it before the report dialog. Before that the run looked
  successful although it had stopped at the first locked file. The files deleted up to that point
  stay deleted.
- **`Config.xml` is copied with `CopyToOutputDirectory=Always`.** A build overwrites the file in
  `bin`, and the installer overwrites an existing `Config.xml` next to an installed executable, so
  an update throws away the configuration of the user. That is how it has always shipped.
- **The configuration is read next to the executable.** `LoadConfig` uses
  `Assembly.GetExecutingAssembly().Location`, so the file has to sit in the installation folder, not
  in `%AppData%`. The current working directory is irrelevant.
- **The language is picked twice.** `InitializeLanguageManager` sets `de-DE`, then
  `LoadLanguagesToCombo` assigns `SelectedIndex = 0`, which fires
  `ComboBoxLanguageSelectedIndexChanged` and sets the language again from the first entry of
  `GetLanguages()`. The effective startup language therefore depends on the order of that list, not
  on the explicit `de-DE`. Changing the constructor order changes the startup language.
- **The window title carries the GitVersion informational version.** `InitializeCaption` uses
  `Application.ProductName` and `Application.ProductVersion`, so an untagged build shows something
  like `LogCleanClient 1.0.8-1+Branch.master.Sha...`. That is the quickest way to tell which build
  is running.
- **The installer is tracked although `.gitignore` excludes `*.exe`.** `Setup/LogCleanClient-Setup.exe`
  is in the index and has to be committed with `git add -f`. Every release adds a full copy of the
  installer to the history.
- **`PrivilegesRequired` is not set, the quick launch icon is dead weight.** The `quicklaunchicon`
  task is limited to `OnlyBelowVersion: 0,6.1`, so it never applies on a supported Windows.
- **AppVeyor badge without CI in the repository.** `README.md` links an AppVeyor build that is
  configured outside of this repository.
- **`src/LogCleanClient.sln.DotSettings`** is tracked and holds nothing but a ReSharper user
  dictionary (`H_00E4mmer`). Leave it alone.
- **`.gitattributes` sets `* text=auto`**, every rule of the Visual Studio template below it is
  commented out. The screenshots, the icon and the installer are detected as binary by git itself.
  Any binary file that git could misread needs its own rule.

## Releasing

1. Make the change.
2. Add an entry at the top of `Changelog.md` in the existing format:
   `* **Version 1.0.8.0 (2026-08-13)** : Short description.`
3. Set `MyAppVersion` in `Setup/LogCleanClient-Setup.iss` to the same four part version. Keep the
   encoding and CRLF, check the bytes afterwards.
4. Commit that.
5. Tag the commit with the plain version number, no `v` prefix (`1.0.8`, `1.0.7`, ...). The existing
   tags are lightweight tags, create new ones the same way.
6. **Only now** build the installer: run `Setup/build-setup-files.bat` from the `Setup` folder, then
   compile `Setup/LogCleanClient-Setup.iss` with `ISCC.exe`. The tag has to exist first, otherwise
   GitVersion burns a prerelease version such as `1.0.8-2+Branch.master.Sha...` into the shipped
   executable.
7. `git add -f Setup/LogCleanClient-Setup.exe`, commit it as `Updated setup.`.
8. Push the commits and the tag.

The version in the `Changelog.md` has four parts (`1.0.8.0`), the tag has three (`1.0.8`).
GitVersion turns the tag into the assembly version. The history shows the order: tag `1.0.7` sits on
`6c08b26 "Updated Nuget packages, added audit mode, moved to Net9.0."`, the installer commit
`c27c2b7 "Updated setup."` comes after it.

## Git

- **Never amend a commit.** No `git commit --amend`, not for a typo in the message, not to add a
  forgotten file, not even when the commit is still local. Write a follow-up commit instead. The
  release versions come from tags on exact commits, an amended commit leaves its tag pointing at a
  commit that no longer exists in the branch.

## Writing style

- Commit messages are written **in English only**: short, precise subject line, explanatory body
  when needed.
- Code comments and comments in project files such as `.csproj` are **always English**, regardless
  of the language used in the conversation.
- **No em dashes or en dashes** (`—`, `–`), neither in prose, commit messages, code comments nor
  documentation. Use a regular hyphen, comma, colon, parentheses or a separate sentence.
- German texts (documentation, chat replies) always use real umlauts and ß, never ASCII
  transliterations such as `ae`, `oe`, `ue` or `ss`. Identifiers, file names and configuration keys
  stay unchanged where umlauts are technically undesirable.
