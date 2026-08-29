#!/usr/bin/env sh
set -eu

script_directory=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
cd "$script_directory"

usage() {
  printf '%s\n' \
    "Usage: ./ahova-bridge <open|doctor|storage|status|logs|start|stop|restart|update|backup|uninstall>" \
    "" \
    "  open       Open the local setup and status page" \
    "  doctor     Check Docker, configuration, process health, and readiness" \
    "  storage    Choose the family folder without editing .env" \
    "  backup     Create a protected Bridge credential/state backup" \
    "  uninstall  Remove the container but keep state and family files" \
    "             Add --purge-local-state to also remove the credential volume"
}

command_name="${1:-}"
case "$command_name" in
  open)
    if command -v open >/dev/null 2>&1; then open http://127.0.0.1:7433
    elif command -v xdg-open >/dev/null 2>&1; then xdg-open http://127.0.0.1:7433
    else printf '%s\n' "Open http://127.0.0.1:7433"
    fi
    ;;
  doctor)
    command -v docker >/dev/null 2>&1 || { printf '%s\n' "✗ Docker is not installed"; exit 1; }
    docker compose version >/dev/null 2>&1 || { printf '%s\n' "✗ Docker Compose v2 is unavailable"; exit 1; }
    docker compose config --quiet
    printf '%s\n' "✓ Docker and configuration"
    docker compose ps bridge
    if command -v curl >/dev/null 2>&1 && curl --fail --silent http://127.0.0.1:7433/health/live >/dev/null; then
      printf '%s\n' "✓ Bridge process is healthy"
    else
      printf '%s\n' "✗ Bridge process is not answering on localhost:7433"
      exit 1
    fi
    if curl --fail --silent http://127.0.0.1:7433/health/ready >/dev/null; then
      printf '%s\n' "✓ Pairing, Ahova contact, and enabled capabilities are ready"
    else
      printf '%s\n' "! Setup is incomplete or an enabled capability needs attention"
      printf '%s\n' "  Open http://127.0.0.1:7433 for an actionable check"
    fi
    ;;
  storage)
    storage_path="${2:-}"
    [ -n "$storage_path" ] || {
      printf '%s\n' "Usage: ./ahova-bridge storage /path/to/family-folder"
      exit 1
    }
    mkdir -p "$storage_path"
    storage_path=$(CDPATH= cd -- "$storage_path" && pwd -P)
    temporary_env=$(mktemp "${script_directory}/.env.XXXXXX")
    trap 'rm -f "$temporary_env"' EXIT INT TERM
    if [ -f .env ]; then
      while IFS= read -r line || [ -n "$line" ]; do
        case "$line" in BRIDGE_STORAGE_HOST_PATH=*) continue ;; esac
        printf '%s\n' "$line" >> "$temporary_env"
      done < .env
    fi
    escaped_path=$(printf '%s' "$storage_path" | sed "s/'/\\\\'/g")
    printf "BRIDGE_STORAGE_HOST_PATH='%s'\n" "$escaped_path" >> "$temporary_env"
    chmod 600 "$temporary_env"
    mv "$temporary_env" .env
    trap - EXIT INT TERM
    docker compose up -d --force-recreate bridge
    printf '%s\n' "Family storage folder: ${storage_path}"
    printf '%s\n' "Bridge restarted. Open the setup page and select Save and test."
    ;;
  status) docker compose ps bridge ;;
  logs) docker compose logs --tail "${2:-100}" bridge ;;
  start) docker compose up -d ;;
  stop) docker compose stop bridge ;;
  restart) docker compose restart bridge ;;
  update)
    docker compose pull
    docker compose up -d
    printf '%s\n' "Updated. Run ./ahova-bridge doctor after the first heartbeat."
    ;;
  backup)
    backup_directory="${2:-${script_directory}/backups}"
    mkdir -p "$backup_directory"
    chmod 700 "$backup_directory"
    backup_name="ahova-bridge-state-$(date -u +%Y%m%dT%H%M%SZ).tgz"
    docker compose stop bridge
    trap 'docker compose start bridge >/dev/null 2>&1 || true' EXIT INT TERM
    docker compose run --rm --no-deps -v "${backup_directory}:/backup" \
      --entrypoint sh bridge -c "tar -czf /backup/${backup_name} -C /var/lib/ahova-bridge ."
    docker compose start bridge
    trap - EXIT INT TERM
    chmod 600 "${backup_directory}/${backup_name}" 2>/dev/null || true
    printf '%s\n' "Created ${backup_directory}/${backup_name}"
    printf '%s\n' "This encrypted-by-you archive contains the installation credential. Store it privately."
    ;;
  uninstall)
    if [ "${2:-}" = "--purge-local-state" ]; then
      printf '%s' "Type PURGE to remove the local pairing credential and receipts: "
      read -r confirmation
      [ "$confirmation" = "PURGE" ] || { printf '%s\n' "Cancelled."; exit 1; }
      docker compose down --volumes
      printf '%s\n' "Container and Bridge state removed. Files in family-files were kept."
    else
      docker compose down
      printf '%s\n' "Container removed. Bridge state and family files were kept."
    fi
    ;;
  *) usage; [ -n "$command_name" ] && exit 1 ;;
esac
