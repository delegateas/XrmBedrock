@echo off
setlocal enabledelayedexpansion

REM Script to create a git worktree with Daxif configuration
REM Usage: create-worktree.bat <branch-name> [--existing]

REM Parse arguments
set "BRANCH_NAME=%~1"
set "CREATE_NEW_BRANCH=true"

if "%~1"=="" goto :show_help
if "%~1"=="-h" goto :show_help
if "%~1"=="--help" goto :show_help

if "%~2"=="--existing" set "CREATE_NEW_BRANCH=false"

REM Validate branch name
if "%BRANCH_NAME%"=="" (
    call :print_error "Branch name is required"
    goto :show_help
)

REM Setup paths
set "FEATURE_BRANCH=feature/%BRANCH_NAME%"
set "WORKTREE_PATH=..\%BRANCH_NAME%"
set "SCRIPT_DIR=%~dp0"
set "USERNAME_FILE=%SCRIPT_DIR%src\Tools\Daxif\username.txt"

call :print_info "Starting worktree creation for branch: %FEATURE_BRANCH%"
call :print_info "Worktree location: %WORKTREE_PATH%"

REM Step 1: Git pull
call :print_info "Pulling latest changes..."
git pull
if errorlevel 1 (
    call :print_error "Failed to pull latest changes"
    exit /b 1
)

REM Step 2: Create worktree
call :print_info "Creating git worktree..."
if "%CREATE_NEW_BRANCH%"=="true" (
    call :print_info "Creating new branch: %FEATURE_BRANCH%"
    git worktree add "%WORKTREE_PATH%" -b "%FEATURE_BRANCH%"
    if errorlevel 1 (
        call :print_error "Failed to create worktree with new branch"
        exit /b 1
    )
) else (
    call :print_info "Using existing branch: %FEATURE_BRANCH%"
    git worktree add "%WORKTREE_PATH%" "%FEATURE_BRANCH%"
    if errorlevel 1 (
        call :print_error "Failed to create worktree with existing branch"
        call :print_warning "Make sure the branch '%FEATURE_BRANCH%' exists"
        exit /b 1
    )
)

REM Step 3: Copy username.txt
call :print_info "Copying username.txt to worktree..."
set "TARGET_USERNAME_FILE=%WORKTREE_PATH%\src\Tools\Daxif\username.txt"

if not exist "%USERNAME_FILE%" (
    call :print_warning "Source username.txt not found at: %USERNAME_FILE%"
    call :print_warning "You may need to create it manually in the worktree"
) else (
    REM Ensure target directory exists
    if not exist "%WORKTREE_PATH%\src\Tools\Daxif" mkdir "%WORKTREE_PATH%\src\Tools\Daxif"

    copy /Y "%USERNAME_FILE%" "%TARGET_USERNAME_FILE%" >nul
    if errorlevel 1 (
        call :print_error "Failed to copy username.txt"
        exit /b 1
    )
    call :print_info "username.txt copied successfully"
)

REM Step 4: Run GenerateCSharpContext.fsx
call :print_info "Running GenerateCSharpContext.fsx..."
set "CSHARP_SCRIPT=%WORKTREE_PATH%\src\Tools\Daxif\GenerateCSharpContext.fsx"

if not exist "%CSHARP_SCRIPT%" (
    call :print_error "GenerateCSharpContext.fsx not found at: %CSHARP_SCRIPT%"
    exit /b 1
)

pushd "%WORKTREE_PATH%\src\Tools\Daxif"
dotnet fsi GenerateCSharpContext.fsx
if errorlevel 1 (
    popd
    call :print_error "Failed to run GenerateCSharpContext.fsx"
    exit /b 1
)
popd
call :print_info "GenerateCSharpContext.fsx completed successfully"

REM Step 5: Run GenerateTypeScriptContext.fsx
call :print_info "Running GenerateTypeScriptContext.fsx..."
set "TYPESCRIPT_SCRIPT=%WORKTREE_PATH%\src\Tools\Daxif\GenerateTypeScriptContext.fsx"

if not exist "%TYPESCRIPT_SCRIPT%" (
    call :print_error "GenerateTypeScriptContext.fsx not found at: %TYPESCRIPT_SCRIPT%"
    exit /b 1
)

pushd "%WORKTREE_PATH%\src\Tools\Daxif"
dotnet fsi GenerateTypeScriptContext.fsx
if errorlevel 1 (
    popd
    call :print_error "Failed to run GenerateTypeScriptContext.fsx"
    exit /b 1
)
popd
call :print_info "GenerateTypeScriptContext.fsx completed successfully"

REM Success message
echo.
call :print_info "========================================="
call :print_info "Worktree created successfully!"
call :print_info "========================================="
call :print_info "Branch: %FEATURE_BRANCH%"
call :print_info "Location: %WORKTREE_PATH%"
echo.
call :print_info "To switch to your worktree:"
call :print_info "  cd %WORKTREE_PATH%"
echo.
call :print_info "To remove the worktree later:"
call :print_info "  git worktree remove %WORKTREE_PATH%"
echo.

exit /b 0

:show_help
echo Usage: create-worktree.bat ^<branch-name^> [--existing]
echo.
echo Creates a git worktree for feature development with Daxif configuration.
echo.
echo Arguments:
echo   branch-name    The name of the branch (without 'feature/' prefix)
echo   --existing     Use existing branch instead of creating a new one (optional)
echo.
echo Examples:
echo   create-worktree.bat my-feature        # Creates new branch feature/my-feature
echo   create-worktree.bat my-feature --existing  # Uses existing feature/my-feature
echo.
echo What this script does:
echo   1. Pulls latest changes from git
echo   2. Creates a worktree in ..\branch-name for feature/branch-name
echo   3. Copies username.txt to the worktree
echo   4. Runs GenerateCSharpContext.fsx
echo   5. Runs GenerateTypeScriptContext.fsx
echo.
exit /b 0

:print_info
echo [INFO] %~1
exit /b 0

:print_error
echo [ERROR] %~1
exit /b 0

:print_warning
echo [WARNING] %~1
exit /b 0
