#!/usr/bin/env sh
set -eu

repository="${AHOVA_BRIDGE_WIKI_URL:-https://github.com/IlyaBaikou/Ahova-Bridge.wiki.git}"
source_directory=$(CDPATH= cd -- "$(dirname -- "$0")/../docs/wiki" && pwd)
work_directory=$(mktemp -d)

cleanup() { rm -rf "$work_directory"; }
trap cleanup EXIT INT TERM

git clone "$repository" "$work_directory/wiki"
find "$work_directory/wiki" -mindepth 1 -maxdepth 1 -type f -name '*.md' -delete
cp "$source_directory"/*.md "$work_directory/wiki/"

git -C "$work_directory/wiki" add -- '*.md'
if git -C "$work_directory/wiki" diff --cached --quiet; then
  printf '%s\n' "Ahova Bridge Wiki is already up to date."
  exit 0
fi

git -C "$work_directory/wiki" commit -m "docs: sync wiki from Ahova Bridge"
git -C "$work_directory/wiki" push origin HEAD:master
printf '%s\n' "Published https://github.com/IlyaBaikou/Ahova-Bridge/wiki"
