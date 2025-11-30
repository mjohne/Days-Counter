# CONTRIBUTING

## Coding Standards

- Strictly follow the rules in `.editorconfig` (indentation, newline policy, naming conventions).

- Use the specified C# version (`13.0`) and target framework `.NET 9`.

- Methods, fields, and variables should use PascalCase or camelCase as specified in `.editorconfig`.

- Avoid `Console.WriteLine` in production code; use centralized logging (e.g., `NLog`).

## Logging

- Use centralized, configurable logger instances and an `NLog` configuration (`NLog.config`).

- Always log exceptions in a structured manner using `Logger.Error(exception, "message")` or `Logger.Fatal` for critical errors.

- Do not store sensitive or personal data in logs.


## Error Handling

- Register global handlers for uncaught exceptions:

- `Application.ThreadException` for UI threads.

- `AppDomain.CurrentDomain.UnhandledException` for background threads.

- `TaskScheduler.UnobservedTaskException` for unobserved tasks.

- Catch blocks should be short and only perform logging and safe recovery/termination. No silent "swallowing".

- Show the user only generic error messages; detailed information belongs in the logs.

## Naming Conventions

- Classes: PascalCase

- Methods: PascalCase

- Private fields: `_camelCase` or `camelCase` depending on the `.editorconfig` rule

- Constants: PascalCase

## Tests & Pull Requests

- Write unit tests for business-critical logic and error paths.


- Use mocks for external dependencies when testing.

- Create pull requests with a clear description, minimal, review-friendly diff, and links to issues.

## Telemetry & Monitoring

- Telemetry integrations (e.g., Application Insights) are welcome; be mindful of data privacy and sampling.

- Define clear log levels and alert rules in the operations documentation.

## Tools & IDE

- Adhere to the rules in `.editorconfig`.

- For Visual Studio settings, see Tools > Options.

## Other Notes

- Maintain this file whenever project standards are changed.

- If you are unsure about anything, ask the team first and submit small, incremental changes via pull requests.