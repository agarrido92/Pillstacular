# 💊 Pillstacular

**Medication tracking that respects your routine—and your privacy.**

Pillstacular is a personal medication tracker built with **.NET MAUI and C#**. It aims to make it simple to record what you've taken, see how long it's been since your last dose, and review your medication history—without sending personal medication data to a server.

## Current domain model

| Model | Purpose |
| --- | --- |
| `Medication` | Medication details, optional private nickname, dosage, form, default quantity, as-needed status, and active/inactive state. |
| `MedicationSchedule` | Planned dose times and quantities associated with a medication. |
| `MedicationEntry` | Actual medication intake, including when it was taken and how much. |

Data is stored locally with **SQLite**. Medication records support create, view, edit, and **soft delete**, preserving records for historical tracking.

## Planned screens

- **Home:** Medication intakes from the past 24 hours, showing elapsed time since each dose.
- **My Medications:** Add, edit, deactivate, and reactivate medications.
- **Medication History:** Chronological daily entries with an expandable calendar.
- **Medication Intake:** Record an as-needed dose or confirm a scheduled one.

## Planned features

- Scheduled reminders using a **private nickname** instead of the medication name when configured.
- Quick logging for as-needed medication and confirmation of scheduled doses.
- Editable intake history and a calendar-based view.
- Reactivation of inactive medications without duplicating records.
- CSV export and import for personal backups (import replaces existing data).
- **Local-first privacy:** No account or backend required.

## Tech stack

**.NET MAUI · C# · SQLite**

> **Status:** In development. Medication management CRUD and local persistence are implemented; reminders, intake tracking, and history views are planned.
