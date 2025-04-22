# Project Brief

## Overview
This repository is a base template for code written for Microsoft Dataverse and Azure services, designed for close integration between Dataverse and Azure. It provides a structured foundation for building, testing, and deploying solutions that span both platforms, supporting trunk-based development and automated CI/CD.

## Core Requirements and Goals
- Enable seamless integration between Microsoft Dataverse and Azure services.
- Provide a standardized, maintainable codebase for Dataverse plugins, Azure Functions, and shared logic.
- Support trunk-based development with robust CI/CD pipelines for build, test, and deployment.
- Facilitate one-time setup operations (e.g., certificate generation) and infrastructure-as-code deployments to Azure.
- Ensure testability of business logic spanning Dataverse and Azure, with support for in-memory mocks and service substitution.
- Promote code reuse and separation of concerns via shared projects for domain models, proxy classes, and business logic.

## Project Scope
- Source code for Azure services (latest LTS .NET), Dataverse plugins (both .NET 4.6.2 and latest LTS), and shared logic.
- Front-end resources for Dataverse (JavaScript web resources).
- Infrastructure-as-code (Bicep) for Azure resource provisioning.
- Scripts for setup and configuration.
- F# tooling for automation and code generation.
- Comprehensive test projects for cross-platform business logic.
- CI/CD pipeline definitions for build and deployment.

## Out of Scope
- Direct management of Dataverse environments (beyond deployment of plugins and web resources).
- Non-Azure cloud providers.
- Non-Dataverse CRM platforms.

## Stakeholders
- Developers building solutions on Dataverse and Azure.
- DevOps engineers managing deployment and infrastructure.
- Testers validating business logic across platforms.

## Source of Truth
This document defines the foundational requirements and scope for the repository. All architectural, technical, and process decisions should align with the goals and constraints outlined here.
