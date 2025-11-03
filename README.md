# git-merger

A .NET 9 command-line tool that ports the last commit from every branch of a source Git repository to a target repository, maintaining the same branch structure!

## Table of Contents
- [Prerequisites](#prerequisites)
- [Setup](#setup)
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
- **Windows**: Fully supported (uses Robocopy for file operations)
- **Linux/macOS**: Requires modification (Robocopy is Windows-specific)

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

### 3. Run the Application

#### Option A: Using dotnet run
```bash
cd GitMerger
dotnet run
```

#### Option B: Using the compiled executable
```bash
# After building in Release mode
cd GitMerger/bin/Release/net9.0
dotnet GitMerger.dll

# Or on Windows
GitMerger.exe
```

### 4. Prepare Your Repositories

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

## Limitations

### Platform Limitations
- **Windows Only**: The tool uses `Robocopy` (a Windows-specific utility) for file copying operations. On Linux/macOS, you would need to implement an alternative `ICopyService` that uses platform-appropriate file copy commands.

### Functional Limitations
1. **Last Commit Only**: The tool only ports the **latest commit** from each branch, not the entire commit history.

2. **Branch Structure**: 
   - Creates branches in the target repository matching the source repository's branch names
   - Removes `origin/` prefix from remote branch names
   - Uses "master" as the default root branch (hardcoded)

3. **Git Configuration**:
   - Uses a hardcoded test signature for commits:
     - Name: "test"
     - Email: "test@mail.eu"
   - You should modify `Program.cs` to use appropriate author information

4. **File Operations**:
   - Excludes `.git` directory when copying (as intended)
   - Stages all files (`*`) in the target repository
   - Silently catches and logs commit exceptions

5. **Interactive Console**:
   - Requires manual input for repository paths
   - No command-line argument support
   - Input validation is basic (checks for null/empty strings only)

6. **No Merge Conflict Resolution**:
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

#### 2. Run git-merger

```bash
# Navigate to the built application
cd git-merger/GitMerger/bin/Release/net9.0

# Run the tool
dotnet GitMerger.dll
```

#### 3. Provide Input

The tool will prompt you for three inputs:

```
Cartella repo partenza:
C:\path\to\legacy-project

Cartella repo destinazione:
C:\path\to\new-project

Subdir:
src
```

**Input Explanation:**
- **Cartella repo partenza** (Source repository path): Full path to your source Git repository
- **Cartella repo destinazione** (Target repository path): Full path to your target Git repository
- **Subdir**: Subdirectory within the target repository where files will be placed (e.g., "src", "code", or "." for root)

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

---

## How It Works

### Process Flow

1. **Branch Iteration**: Iterates through all branches in the source repository
2. **Checkout**: Checks out each branch in the source repository
3. **Branch Creation**: Creates corresponding branches in the target repository (if they don't exist)
4. **File Copy**: Uses Robocopy to copy files (excluding `.git`) to the target location
5. **Commit**: Stages all changes and creates a commit in the target repository
6. **Repeat**: Processes all branches sequentially

### Core Components

- **GitMerger.Core**: Library containing the core logic
  - `GitMerger.CloneLastCommitOfAllBranches()`: Main orchestration method
  - `GitMerger.CloneBranch()`: Handles individual branch porting
  - `ICopyService`: Interface for file copy operations
  - `RobocopyService`: Windows-specific file copy implementation

- **GitMerger**: Console application providing the user interface

### Technology Stack

- **.NET 9**: Modern cross-platform framework
- **LibGit2Sharp 0.30.0**: .NET bindings for libgit2 (Git operations)
- **Robocopy**: Windows utility for file copying

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Copyright (c) 2017 phenixita
