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
