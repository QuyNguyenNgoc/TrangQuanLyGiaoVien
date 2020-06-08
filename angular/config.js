﻿// In real applications, you should not transpile code in the browser. You can see how to create your own application with Angular and DevExtreme here:
// https://js.devexpress.com/Documentation/Guide/Angular_Components/Getting_Started/Create_a_DevExtreme_Application/

System.config({
    transpiler: 'ts',
    typescriptOptions: {
        module: "commonjs",
        emitDecoratorMetadata: true,
        experimentalDecorators: true
    },
    meta: {
        'typescript': {
            "exports": "ts"
        }
    },
    paths: {
        'npm:': 'https://unpkg.com/'
    },
    map: {
        'ts': 'npm:plugin-typescript@8.0.0/lib/plugin.js',
        'typescript': 'npm:typescript@3.4.5/lib/typescript.js',

        '@angular/core': 'npm:@angular/core@8.0.0/bundles/core.umd.js',
        '@angular/common': 'npm:@angular/common@8.0.0/bundles/common.umd.js',
        '@angular/compiler': 'npm:@angular/compiler@8.0.0/bundles/compiler.umd.js',
        '@angular/platform-browser': 'npm:@angular/platform-browser@8.0.0/bundles/platform-browser.umd.js',
        '@angular/platform-browser-dynamic': 'npm:@angular/platform-browser-dynamic@8.0.0/bundles/platform-browser-dynamic.umd.js',
        '@angular/router': 'npm:@angular/router@8.0.0/bundles/router.umd.js',
        '@angular/forms': 'npm:@angular/forms@8.0.0/bundles/forms.umd.js',
        '@angular/common/http': 'npm:@angular/common@8.0.0/bundles/common-http.umd.js',
        'tslib': 'npm:tslib/tslib.js',

        'rxjs': 'npm:rxjs@6.3.3',
        'rxjs/operators': 'npm:rxjs@6.3.3/operators',

        'devextreme': 'npm:devextreme@19.2',
        'jszip': 'npm:jszip@3.1.3/dist/jszip.min.js',
        'quill': 'npm:quill@1.3.7/dist/quill.js',
        'devexpress-diagram': 'npm:devexpress-diagram',
        'devexpress-gantt': 'npm:devexpress-gantt@0.0.8/dist/dx-gantt.js',
        'devextreme-angular': 'npm:devextreme-angular@19.2'
    },
    packages: {
        'app': {
            main: './app.component.ts',
            defaultExtension: 'ts'
        },
        'devextreme': {
            defaultExtension: 'js'
        },
        'rxjs': { main: 'index.js', defaultExtension: 'js' },
        'rxjs/operators': { main: 'index.js', defaultExtension: 'js' }
    },
    packageConfigPaths: [
        "npm:*/package.json",
        "npm:@angular/*/package.json",
        "npm:@angular/common/*/package.json",
        "npm:rxjs/operators/package.json",
        "npm:devextreme-angular/*/package.json",
        "npm:devextreme-angular/ui/*/package.json"
    ]
});