# Draft translations

`drafts/zh-Hans.mjs` contains the complete 133-entry Simplified Chinese candidate for
`src/Ahova.Bridge/wwwroot/index.html`. It is outside the served web root and is not imported by the
runtime. English, Russian and Belarusian remain the supported setup languages.

The draft preserves the dynamic `foundModels(n)` and `pairedWith(name)` arguments, setup status keys,
HTML emphasis in the pairing guide, protocol names and numeric limits. It was checked against the
current English key set, including counts 0, 1, 2, 5, 11 and 21.

Native-speaker and actual layout review are pending. Integrate the reviewed catalogue into the existing
translation map and language selector alongside the app, website and Server release; do not advertise
Chinese solely because this file exists. The full pilot report is in the HomeOS companion repository:
`localization/audit/chinese-draft-2026-09-05.md`.

Verification on 5 September 2026: formatting, existing EN/RU/BE localization checks, 25 .NET tests,
draft catalogue parity/function checks and Docker build passed. The Mac was locked when native UI
review was attempted, so no Chinese visual acceptance is claimed.
