# Folder Structure

1. package.json same as node's page format.
2. `bin` folder contains additional dlls.
3. All javascript files can only access dll specified in `bin` folder of root package.
4. While resolving dependent packages, bin folder is populated from referenced package's bin folder.