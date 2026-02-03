# Security & Secrets Handling

This project ships **code only**. All platform credentials, signing keys, and production identifiers must be supplied locally or via CI secrets and must **not** live in the repository.

## What to keep out of git
- Platform credentials (PS4 passcodes, keystores, provisioning profiles, signing keys)
- Service IDs/keys (Unity Services, Google Play Games, Firebase, analytics, ad networks)
- Personal data or real player saves
- Proprietary art/audio/design source files

## How to provide secrets locally
1. Store platform keys outside the repo (e.g., `~/secrets/keystores/user.keystore`).
2. Configure Unity Services/Firebase IDs via the Unity Editor or environment variables in CI.
3. For builds in CI, inject secrets as environment variables or mounted files; do not commit them.
4. Current public build is offline-only; you may leave all service IDs unset for local playtests.

## Rotation & validation
- Replace placeholders in `ProjectSettings/ProjectSettings.asset` and `Assets/Plugins/Android/GooglePlayGamesManifest.androidlib/AndroidManifest.xml` only in private branches.
- Rotate any leaked credentials immediately; delete the secret, regenerate, and update CI secrets.

## Reporting
If you discover a security issue, please open a private issue/email with details instead of filing a public issue. Include reproduction steps and affected versions.
