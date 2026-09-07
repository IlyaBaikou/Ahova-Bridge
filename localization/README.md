# Bridge localization

The setup interface supports English, Russian, Belarusian, Simplified Chinese (`zh-Hans`), German,
Spanish, Brazilian Portuguese (`pt-BR`), French, Italian, Turkish, Polish, Ukrainian, Korean and Japanese.
Its 133-entry catalogues live in `src/Ahova.Bridge/wwwroot/index.html`; dynamic values,
setup checks and configuration errors are verified by `node scripts/verify-localization.mjs`.

The Chinese pilot was prepared on 5 September 2026 and integrated on 6 September at the
owner’s explicit request to enable all surfaces and publish. Native-speaker review remains
pending; runtime availability does not claim that review has happened. The former standalone
draft was moved into the served catalogue to avoid duplicate translation sources.

Chinese browser tags `zh`, `zh-CN`, `zh-SG` and `zh-Hans` resolve to Simplified Chinese.
Traditional Chinese preferences fall back to English; the user can select Simplified Chinese.
The chosen language persists. Provider names, commands and family-authored text are preserved.
Headings use platform fonts for Chinese because the editorial faces lack Han coverage.

Validation: catalogue/setup-error checks, .NET formatting, 25 tests and Docker build passed.
The wizard’s Chinese language selection and first step were visually inspected in the browser.
This is not clean-machine pairing or model/storage interoperability evidence.

The companion HomeOS report is `localization/audit/chinese-draft-2026-09-05.md`.

## Global release — 7 September 2026

The owner requested all languages across the app, website, Server and Bridge, including replacement
of European Portuguese with Brazilian Portuguese. The ten added catalogues now live in the served
HTML. Standalone drafts were removed. Portuguese browser preferences resolve to `pt-BR`.

The verifier checks all 14 served catalogues through their real runtime helpers: 133 keys, setup
states, configuration failures, dynamic counts/names, language aliases and the pairing breadcrumb.
Chinese, Korean and Japanese headings use platform fonts. Provider names, commands, credentials,
limits and HTML structure retain their meaning.

Catalogue checks, .NET formatting, all 25 tests and the container build passed. Independent
native-speaker review and actual provider/pairing flows in these languages remain pending.
Machine-assisted drafts received contextual corrections; owner-authorized runtime availability
does not certify native review. See the companion HomeOS report
`localization/audit/global-languages-2026-09-07.md` for evidence and release limits.
