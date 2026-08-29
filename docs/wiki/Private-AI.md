# Private AI

Ahova Bridge can process supported requests with a model running on a trusted computer or home server.
Local processing uses zero Smart Actions. Ahova still applies authorization, grounded context,
structured-output validation, and action confirmation.

## Supported model servers

| Server | Simple setup | Advanced endpoint |
| --- | --- | --- |
| Ollama | Bridge finds pulled models | `http://host.docker.internal:11434/v1/` |
| LM Studio | Bridge finds loaded models | `http://host.docker.internal:1234/v1/` |
| Another compatible server | enter its address under **Advanced** | its OpenAI-compatible `/v1/` URL |

The endpoint must be reachable from the Bridge container. `localhost` inside Docker means Bridge itself,
so a server on the Docker host normally uses `host.docker.internal`.

## Ollama

1. Install and start [Ollama](https://ollama.com/).
2. Pull a recent instruction-tuned model with reliable multilingual and structured JSON output:

   ```bash
   ollama pull qwen3:8b
   ```

3. In the Bridge wizard enable **Use my private AI**.
4. Select **Find local AI**.
5. Choose the model Bridge discovered.
6. Select **Save and test**.

On Linux, a host service bound only to `127.0.0.1` may not be reachable from Docker. Configure Ollama to
listen on the Docker host interface, protect the port with the host firewall, and never expose an
unauthenticated model server publicly.

## LM Studio

1. Install [LM Studio](https://lmstudio.ai/).
2. Download and load a model.
3. Start the local OpenAI-compatible server.
4. Enable private AI in Bridge and select **Find local AI**.
5. Choose the loaded model and select **Save and test**.

Bridge expects the local endpoint to be reachable without an API key from its container. Keep it on a
trusted host or private network. Do not put an inference-service key into an unrelated Bridge field.

## Custom endpoint or several models

Enable **Advanced** to:

- select Ollama or OpenAI-compatible mode explicitly;
- enter a custom endpoint;
- maintain a comma-separated model allowlist.

Run discovery again after entering a custom endpoint. Model identifiers must exactly match those
reported by Ollama or `/v1/models`.

## Choosing a model

Prefer a recent instruction-tuned model that:

- understands all family languages you use;
- reliably returns structured JSON;
- fits available RAM or VRAM without excessive swapping;
- completes a representative request within Bridge's bounded timeout.

Start with one model and validate real family examples before adding another. A smaller reliable model
is usually better than a larger model that times out.

## Verify the full path

1. Confirm the local wizard shows private AI **Ready**.
2. Run `./ahova-bridge doctor` or `.\ahova-bridge.ps1 doctor`.
3. Confirm the Ahova Bridge page reports recent contact.
4. Submit a small supported Ahova request and review the proposal before saving it.
5. Confirm the Smart Action balance did not decrease.

Ahova does not silently spend a managed Smart Action if Bridge or the model fails.

## Common problems

- **No local AI found:** start the model server, then try discovery again.
- **Connection refused:** replace `localhost` with `host.docker.internal`; on Linux check binding and
  firewall rules.
- **No models listed:** pull an Ollama model or load one in LM Studio.
- **Model not allowed:** run discovery again and reselect the exact identifier.
- **Invalid structured result:** use a stronger instruction-tuned model with better JSON support.
- **Slow or timed out:** choose a smaller model and check host load and available memory.

See [Troubleshooting](Troubleshooting) for safe diagnostics and recovery steps.
