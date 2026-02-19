# Copilot Instructions

## Code Comments

- Do not add comments to generated code unless the code does something unintuitive or requires explanation.
- Never remove existing comments from the code.

## Terminal

- The terminal is **PowerShell**. Never use bash-only commands such as `grep`, `tail`, `cat` with heredoc syntax, etc. Use PowerShell equivalents (`Select-String`, `Select-Object`, `Set-Content`, etc.).

## CSS

- Prefer scoped `.razor.css` files co-located with the component over adding styles to `app.css`. Only use `app.css` for truly global styles (resets, typography, layout, utilities).

## Development Server

- The application is almost always already running via `run.bat` in a terminal. Avoid starting a new instance unless you have confirmed none is running.
