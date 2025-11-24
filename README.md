# git-merger

A .NET 9 command-line tool that ports the last commit from every branch of a source Git repository to a target repository, maintaining the same branch structure.

**New in this version:** Configuration-driven setup with JSON config files and command-line arguments for non-interactive, automated usage!

## Table of Contents
- [Prerequisites](#prerequisites)
- [Setup](#setup)
- [Usage](#usage)
  - [Quick Start](#quick-start)
  - [Configuration File](#configuration-file)
  - [Command-Line Arguments](#command-line-arguments)
  - [Interactive Mode](#interactive-mode)
- [Configuration Options](#configuration-options)
- [Limitations](#limitations)
- [Use Case Example](#use-case-example)
- [How It Works](#how-it-works)
- [License](#license)

---

## Prerequisites

Before using git-merger, ensure you have the following installed:

### Required Software
- **.NET 9 SDK** (or later)
  - Download from: https://dotnet.microsoft.com/download/dotnet/9.0
  - Verify installation: `dotnet --version`

- **Git** (version 2.0 or later)
  - Download from: https://git-scm.com/downloads
  - Verify installation: `git --version`

### Platform Requirements
- **Windows**: Fully supported (uses Robocopy for file operations by default)
- **Linux/macOS**: Fully supported (use `--copy-service systemio` for cross-platform file operations)

### Knowledge Requirements
- Basic understanding of Git concepts (branches, commits, repositories)
- Familiarity with command-line interfaces

---

## Setup

### 1. Clone or Download the Repository

```bash
git clone https://github.com/phenixita/git-merger.git
cd git-merger
```

### 2. Build the Project

```bash
# Restore dependencies and build
dotnet build GitMerger.sln --configuration Release

# Or build in debug mode
dotnet build GitMerger.sln
```

### 3. Prepare Your Repositories

Before running the tool, you need:

1. **Source Repository**: An existing Git repository with branches you want to port
2. **Target Repository**: An existing Git repository (can be empty or initialized with `git init`)

```bash
# Example: Create a target repository
mkdir target-repo
cd target-repo
git init
cd ..
```

---

## Usage

Git-merger now supports three modes of operation:

### Quick Start

```bash
# Generate an example configuration file
cd GitMerger/bin/Release/net9.0
dotnet GitMerger.dll --init-config

# Edit the generated gitmerger.json with your settings
# Then run with the configuration
dotnet GitMerger.dll --config gitmerger.json
```

### Configuration File

Create a `gitmerger.json` file with your settings:

```json
{
  "SourceRepo": "/path/to/source",
  "TargetRepo": "/path/to/target",
  "Subdir": "src",
  "Author": {
    "Name": "Your Name",
    "Email": "your@email.com"
  },
  "RootBranch": "main",
  "StagePatterns": ["*"],
  "CopyService": "systemio"
}
```

**Configuration Options:**
- `SourceRepo`: Full path to the source Git repository
- `TargetRepo`: Full path to the target Git repository
- `Subdir`: Subdirectory within the target repository where files will be placed
- `Author.Name`: Name for commit author
- `Author.Email`: Email for commit author
- `RootBranch`: The root branch name (default: "master")
- `StagePatterns`: Array of file patterns to stage (default: ["*"])
- `CopyService`: Copy service to use - "robocopy" (Windows) or "systemio" (cross-platform)

Then run:

```bash
dotnet GitMerger.dll --config gitmerger.json
```

### Command-Line Arguments

Override any configuration value using command-line arguments:

```bash
# Use specific paths
dotnet GitMerger.dll -s /path/to/source -t /path/to/target -d src

# With custom author information
dotnet GitMerger.dll \
  -s /path/to/source \
  -t /path/to/target \
  -d src \
  -n "John Doe" \
  -e "john@example.com"

# For Linux/macOS, use the cross-platform copy service
dotnet GitMerger.dll \
  -s /path/to/source \
  -t /path/to/target \
  -d src \
  -c systemio

# Use a different root branch
dotnet GitMerger.dll \
  --config myconfig.json \
  --root-branch main
```

**Command-Line Options:**
- `--config <path>` or `-cfg`: Load configuration from JSON file
- `--source <path>` or `-s`: Source repository path
- `--target <path>` or `-t`: Target repository path
- `--subdir <path>` or `-d`: Subdirectory in target repository
- `--author-name <name>` or `-n`: Author name for commits
- `--author-email <email>` or `-e`: Author email for commits
- `--root-branch <name>` or `-b`: Root branch name
- `--copy-service <type>` or `-c`: Copy service type (robocopy or systemio)
- `--init-config [path]`: Create example configuration file
- `--help` or `-h`: Show help message

### Interactive Mode

If you don't provide all required values via config file or command-line arguments, the tool will prompt you interactively:

```bash
dotnet GitMerger.dll
```

You'll be prompted for:
1. Source repository path
2. Target repository path
3. Subdirectory

---

## Configuration Options

### Copy Services

Git-merger supports two copy services:

1. **robocopy** (Windows-only): Uses the Windows Robocopy utility for file operations
   - Fast and efficient on Windows
   - Automatically excludes `.git` directory
   
2. **systemio** (Cross-platform): Uses .NET's System.IO for file operations
   - Works on Windows, Linux, and macOS
   - Pure .NET implementation

Choose the appropriate copy service based on your platform:
- Windows: Use either `robocopy` (default) or `systemio`
- Linux/macOS: Use `systemio`

### Stage Patterns

Control which files are staged and committed using the `StagePatterns` configuration:

```json
{
  "StagePatterns": ["*"]  // Stage all files (default)
}
```

You can specify multiple patterns:

```json
{
  "StagePatterns": ["*.cs", "*.csproj"]  // Only stage C# files
}
```

---

## Limitations

### Functional Limitations
1. **Last Commit Only**: The tool only ports the **latest commit** from each branch, not the entire commit history.

2. **Branch Structure**: 
   - Creates branches in the target repository matching the source repository's branch names
   - Removes `origin/` prefix from remote branch names

3. **File Operations**:
   - Excludes `.git` directory when copying (as intended)
   - Stages files based on configured patterns (default: all files)
   - Silently catches and logs commit exceptions

4. **No Merge Conflict Resolution**:
   - If conflicts arise during the porting process, they must be resolved manually
   - The tool doesn't provide conflict resolution mechanisms

### Technical Limitations
- Requires both repositories to be valid Git repositories
- Requires write access to the target repository
- May fail if branch names contain special characters
- No progress indication for large repositories

---

## Use Case Example

### Scenario
You have a large legacy repository with multiple feature branches. You want to create a new repository that contains only the latest state of each branch (without the full history) to:
- Reduce repository size
- Start fresh with a clean history
- Migrate to a new structure while preserving branch organization

### Step-by-Step Example

#### 1. Setup Repositories

**Source Repository Structure:**
```
legacy-project/
├── master (latest commit: "Update README")
├── feature/authentication (latest commit: "Add OAuth support")
└── feature/database (latest commit: "Optimize queries")
```

**Create Target Repository:**
```bash
# Create and initialize target repository
mkdir new-project
cd new-project
git init
cd ..
```

#### 2. Create Configuration File

Generate an example configuration:
```bash
cd git-merger/GitMerger/bin/Release/net9.0
dotnet GitMerger.dll --init-config
```

Edit `gitmerger.json`:
```json
{
  "SourceRepo": "/path/to/legacy-project",
  "TargetRepo": "/path/to/new-project",
  "Subdir": "src",
  "Author": {
    "Name": "John Doe",
    "Email": "john@example.com"
  },
  "RootBranch": "master",
  "StagePatterns": ["*"],
  "CopyService": "systemio"
}
```

#### 3. Run git-merger

```bash
# Using configuration file
dotnet GitMerger.dll --config gitmerger.json

# Or using command-line arguments
dotnet GitMerger.dll \
  -s /path/to/legacy-project \
  -t /path/to/new-project \
  -d src \
  -n "John Doe" \
  -e "john@example.com" \
  -c systemio
```

#### 4. Result

**Target Repository Structure After Porting:**
```
new-project/
├── master
│   └── src/
│       └── [files from legacy-project's master branch]
├── feature/authentication
│   └── src/
│       └── [files from legacy-project's feature/authentication branch]
└── feature/database
    └── src/
        └── [files from legacy-project's feature/database branch]
```

Each branch in `new-project` contains:
- A commit with message: "Import [branch-name]"
- The latest files from the corresponding branch in `legacy-project`
- Files placed in the specified subdirectory (`src` in this example)

#### 5. Verify the Results

```bash
cd new-project

# Check branches were created
git branch -a

# Review a specific branch
git checkout feature/authentication
git log --oneline
git ls-files
```

### Real-World Use Cases

1. **Repository Splitting**: Extract specific branches from a monorepo into a separate repository
2. **Clean History**: Start a new repository with current branch states, leaving behind messy history
3. **Archive Migration**: Create snapshots of branch states for archival purposes
4. **Code Organization**: Restructure files into subdirectories during the migration
5. **Branch Consolidation**: Bring together branches from multiple source repositories
6. **CI/CD Integration**: Automate repository migrations in build pipelines using configuration files

---

## How It Works

### Process Flow

1. **Configuration Loading**: Loads settings from JSON file (if exists)
2. **Argument Parsing**: Overrides config values with command-line arguments
3. **Interactive Prompting**: Prompts for any missing required values
4. **Validation**: Validates that repositories exist and configuration is valid
5. **Branch Iteration**: Iterates through all branches in the source repository
6. **Checkout**: Checks out each branch in the source repository
7. **Branch Creation**: Creates corresponding branches in the target repository (if they don't exist)
8. **File Copy**: Uses the configured copy service to copy files (excluding `.git`) to the target location
9. **Commit**: Stages configured patterns and creates a commit in the target repository
10. **Repeat**: Processes all branches sequentially

### Core Components

- **GitMerger.Core**: Library containing the core logic
  - `GitMerger.CloneLastCommitOfAllBranches()`: Main orchestration method
  - `GitMerger.CloneBranch()`: Handles individual branch porting
  - `ICopyService`: Interface for file copy operations
  - `RobocopyService`: Windows-specific file copy implementation
  - `SystemIOCopyService`: Cross-platform file copy implementation
  - `CopyServiceFactory`: Creates appropriate copy service based on configuration

- **GitMerger**: Console application providing the user interface
  - `Configuration`: Configuration models and loaders
  - `CommandLineParser`: Parses command-line arguments
  - `ConfigurationLoader`: Loads and saves JSON configuration files

### Technology Stack

- **.NET 9**: Modern cross-platform framework
- **LibGit2Sharp 0.30.0**: .NET bindings for libgit2 (Git operations)
- **System.Text.Json**: Built-in JSON serialization (for configuration)
- **Robocopy**: Windows utility for file copying (optional)
- **System.IO**: .NET file operations (cross-platform alternative)

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Copyright (c) 2017 phenixita
