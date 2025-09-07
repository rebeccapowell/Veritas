# Documentation Setup

This repository uses [DocFX](https://dotnet.github.io/docfx/) to build API documentation and publish the static site to GitHub Pages. A GitHub Actions workflow at `.github/workflows/docs.yml` runs on every release to generate and deploy the docs.

## Initial setup

1. Ensure a DocFX configuration exists at `docfx/docfx.json`. If you need a starting point, run `docfx init -q` and customize the generated files.
2. Enable GitHub Pages:
   - Go to **Settings → Pages**.
   - Under **Build and deployment**, choose **Deploy from a branch**.
   - Select the `gh-pages` branch and `/` (root) folder.

After publishing a release, the workflow will build the docs with DocFX and publish them to GitHub Pages automatically.

For a detailed walkthrough of this process, see [Steven Giesel's blog post](https://steven-giesel.com/blogPost/5f9e9f0d-2413-4e4b-8e38-9eebe9503e52).
