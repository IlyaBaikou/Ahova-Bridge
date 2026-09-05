# Bridge localization

The setup interface supports English, Russian, Belarusian and Simplified Chinese (`zh-Hans`).
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

## European drafts — 6 September 2026

German, Spanish, European Portuguese (`pt-PT`) and French have complete 133-entry drafts in
`drafts/{de,es,pt,fr}.mjs`. They use the same terminology as the companion mobile app, website and
Server console. Provider names, commands, credentials, limits, HTML structure and dynamic values
retain their meaning. Dynamic model counts use CLDR-aware plural rules, including the European
Portuguese rule for zero.

The existing localization verifier injects these catalogues into its isolated wizard context to
check every setup state, configuration failure, dynamic name/count and the pairing breadcrumb.
Drafts are not loaded by the served HTML and are not exposed by language selection or detection.
No production release of the four new languages is implied.

Catalogue checks, .NET formatting, all 25 tests and the container build passed. Fluent-speaker review,
full wizard layout review and actual provider/pairing flows in these languages remain pending.
See the companion HomeOS report `localization/audit/european-locales-2026-09-06.md` for the full
19,204-entry cross-product scope and release boundary.
