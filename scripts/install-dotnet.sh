#!/usr/bin/env bash
set -euo pipefail

readonly DOTNET_VERSION="10.0.400"
readonly DOTNET_DIRECTORY="${PWD}/.dotnet"
readonly INSTALL_SCRIPT="/tmp/nat-myat-si-dotnet-install.sh"

if [[ ! -x "${DOTNET_DIRECTORY}/dotnet" ]]; then
    curl --fail --silent --show-error --location \
        "https://dot.net/v1/dotnet-install.sh" \
        --output "${INSTALL_SCRIPT}"

    bash "${INSTALL_SCRIPT}" \
        --version "${DOTNET_VERSION}" \
        --install-dir "${DOTNET_DIRECTORY}" \
        --no-path
fi

"${DOTNET_DIRECTORY}/dotnet" --info
