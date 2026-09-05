import assert from 'node:assert/strict';
import fs from 'node:fs';
import vm from 'node:vm';

const html = fs.readFileSync(new URL('../src/Ahova.Bridge/wwwroot/index.html', import.meta.url), 'utf8');
const script = html.match(/<script>([\s\S]*?)<\/script>/)[1].split('const preferred=')[0];
const context = vm.createContext({ document: { getElementById: () => ({ addEventListener() {} }), querySelectorAll: () => [] } });
vm.runInContext(script, context);
const keys = vm.runInContext('Object.keys(translations.en)', context);
// Drafts are tested with the real wizard helpers but remain outside the served page.
const drafts = ['de', 'es', 'pt', 'fr'];
for (const locale of drafts) {
  const { default: catalogue } = await import(`../localization/drafts/${locale}.mjs`);
  assert.deepEqual(Object.keys(catalogue).sort(), [...keys].sort(), `${locale}: draft key parity`);
  context.draftCatalogue = catalogue;
  vm.runInContext(`translations['${locale}'] = draftCatalogue`, context);
  for (const key of keys) {
    assert.equal(typeof catalogue[key], vm.runInContext(`typeof translations.en['${key}']`, context), `${locale}.${key}: value kind`);
  }
  for (const count of [0, 1, 2, 5, 11, 21, 1000000]) {
    assert.ok(catalogue.foundModels(count).startsWith(`${count} `), `${locale}: model count preserved`);
    assert.doesNotMatch(catalogue.foundModels(count), /undefined|NaN/);
  }
  assert.match(catalogue.pairedWith('Test family'), /Test family/);
  assert.deepEqual(catalogue.pairLead.match(/<\/?[^>]+>/g), ['<strong>', '</strong>']);
}
const references = [...html.matchAll(/data-i18n(?:-html|-aria|-placeholder)?="([^"]+)"/g)].map(match => match[1]);
for (const locale of ['en', 'ru', 'be', 'zh-Hans', ...drafts]) {
  vm.runInContext(`currentLanguage='${locale}'`, context);
  assert.equal(vm.runInContext('Object.keys(translations[currentLanguage]).length', context), keys.length);
  for (const key of new Set([...keys, ...references])) {
    assert.equal(vm.runInContext(`Object.hasOwn(translations[currentLanguage], ${JSON.stringify(key)})`, context), true, `${locale}.${key}`);
    assert.ok(vm.runInContext(`String(t(${JSON.stringify(key)}, 2)).trim()`, context));
  }
  for (const [id, states] of Object.entries(vm.runInContext('setupCheckCopy', context))) {
    for (const state of Object.keys(states)) {
      const result = vm.runInContext(`checkPresentation(${JSON.stringify({ id, state, summary: 'untranslated-server-message', action: 'untranslated-server-action' })})`, context);
      assert.ok(result.summary);
      assert.doesNotMatch(result.summary + result.action, /untranslated-server/);
      if (locale === 'zh-Hans') assert.match(result.summary, /\p{Script=Han}/u);
      else if (['ru', 'be'].includes(locale)) assert.match(result.summary, /[А-Яа-яЁёІіЎў]/);
    }
  }
  const unknown = vm.runInContext("checkPresentation({id:'future',state:'new',summary:'private payload'})", context);
  assert.doesNotMatch(unknown.summary, /private payload|future/);
  assert.doesNotMatch(vm.runInContext("configurationError('Unknown field with private payload')", context), /private payload/);
  assert.notEqual(vm.runInContext("configurationError('WebDAV username and password are required.')", context), 'WebDAV username and password are required.');
}
// Every editable backend validation failure must have a useful localized explanation.
const rules = fs.readFileSync(new URL('../src/Ahova.Bridge/Configuration/BridgeOptions.cs', import.meta.url), 'utf8');
const editable = [...rules.matchAll(/errors.Add\("([^"]+)"\)/g)].map(match => match[1])
  .filter(value => !/StateDirectory|PollIntervalSeconds|HeartbeatIntervalSeconds|MaximumTimeoutSeconds|MaximumOutputTokens/.test(value));
for (const error of editable) {
  context.validationError = error;
  assert.notEqual(vm.runInContext('configurationError(validationError)', context), vm.runInContext("t('configurationInvalid')", context), error);
}
console.log(`Bridge localization verified: ${keys.length} keys in EN/RU/BE/ZH and DE/ES/PT/FR drafts, setup states and configuration failures.`);

for (const tag of ['zh','zh-CN','zh-SG','zh-Hans','zh-Hans-CN']) assert.equal(vm.runInContext(`resolveLanguage('${tag}')`,context), 'zh-Hans');
for (const tag of ['zh-TW','zh-Hant','zh-HK']) assert.equal(vm.runInContext(`resolveLanguage('${tag}')`,context), 'en');
