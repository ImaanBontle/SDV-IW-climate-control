# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased](https://github.com/ImaanBontle/SDV-IW-climate-control/compare/v0.7.0...HEAD)

### Added

- Consistent weathers on save-load
- Different daily probabilities
- Config option for enabling/disabling interpolation
- Config option for debug logging

### Fixed

- Incorrect logic for weather checks in Spring 1-4

### Planned

## [v0.7.0](https://github.com/ImaanBontle/SDV-IW-climate-control/compare/v0.6.0...v0.7.0) - 2022-12-14

### What Changed 🚀

Generic Mod Config Menu added!

### ⚠️ Changes

- Tweaked rain probabilities in the standard model for a smoother Summer and Fall (#36)

### 👽️ Integrations

- Generic Mod Config Menu now supported (#29)

**Full Changelog**: https://github.com/ImaanBontle/SDV-IW-climate-control/compare/v0.6.0...v0.7.0

## [v0.6.0](https://github.com/ImaanBontle/SDV-IW-climate-control/compare/0.5.1...v0.6.0) - 2022-12-13

### What Changed 🚀

Granular seasons and bugfixes!

*(NB: Make sure to delete the old config.json and model files when updating.)*

### ✨ New Features

- Seasons are now granular (players can adjust early, mid and late probabilities per season) (#23)

### 🐛 Bug Fixes

- Winter 14, 15 and 16 are always sunny and will be ignored when changing the weather (#24)

### 📄 Documentation

- Switched to an improved changelog system (#21)

**Full Changelog**: https://github.com/ImaanBontle/SDV-IW-climate-control/compare/0.5.1...v0.6.0

## [v0.5.1](https://github.com/ImaanBontle/SDV-IW-climate-control/compare/0.5.0...0.5.1)

- Added XML comments to methods and classes.
- Cleaned up code and simplified config set-up.
- Changed the choice of weather tomorrow from operating on a successful first-come, first-serve basis to a lowest successful dice-roll basis. This means that no weather type is prioritised more than another; only the weather probabilities affect the outcome.
- Added dice-rolls for each successful weather type to the SMAPI trace log.

## [v0.5.0](https://github.com/ImaanBontle/SDV-IW-climate-control/compare/0.4.0...0.5.0)

- Added trace messages to SMAPI log for easier debugging.

## [v0.4.0](https://github.com/ImaanBontle/SDV-IW-climate-control/compare/0.3.1...0.4.0)

- Implemented improved messaging system from framework.
- Now broadcasts all relevant information to framework when changing weather and not just a simple string.
- Waits for permission from framework before recaching the model on save load.

## [v0.3.1](https://github.com/ImaanBontle/SDV-IW-climate-control/compare/0.3.0...0.3.1)

- Stopped mod from caching model every day. Now checks model choice on save loaded.

## [v0.3.0](https://github.com/ImaanBontle/SDV-IW-climate-control/compare/0.2.1...0.3.0)

- Added integration with Framework. Now broadcasts weather changes to SMAPI console through the Framework.
- Created a class framework for storing multiple different template models and printing these to json files at first launch. Major code re-write to accomodate this.
- If player edits a template's json files, mod will use the edited values.
- Player can also create a custom template by editing config.json and also specifying a "custom" model.
- Subcategorised seasonal weather into "Mid" grouping, in anticipation of "Early", "Mid" and "Late" chances of each weather type.
- Fleshed out code comments.
- Added list of models to API.
- Made sure mod only caches Framework's API once rather than daily.
- Mod only checks for model choice the first time.
- Integrated TODO into code, rather than separate file

## [v0.2.1](https://github.com/ImaanBontle/SDV-IW-climate-control/compare/0.2.0...0.2.1)

- Minor code clean-up.

## [v0.2.0](https://github.com/ImaanBontle/SDV-IW-climate-control/compare/0.1.0...0.2.0)

- Added config file for each weather type's chance per season.
- Implemented a dice roll for changing the weather tomorrow (first weather feature implemented!).

## v0.1.0

- Initial release for purposes of generating GitHub keys. Demonstrates rudimentary fixing of weather to a certain day of the week.
