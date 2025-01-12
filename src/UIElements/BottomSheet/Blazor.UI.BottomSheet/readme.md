# BottomSheet in Blazor

## Setup
First, we'll need to create a .NET solution and the blazor wasm project and add tailwindcss to it.

### Creating the .NET solution
1. Run `dotnet new sln -n Blazor.UI` and `cd` to the solution folder
2. Then `dotnet new blazorwasm-empty -o src/Blazor.UI.BottomSheet`
3. Add the project to the solution: `dotnet sln add src/Blazor.UI.BottomSheet`


### Adding tailwindcss
1. `cd` to the blazor folder
2. Run `npx tailwindcss init`
3. Edit the `tailwind.config.js` content property with the following `['./**/*.{razor,cshtml,html}']`
4. Add a `wwwroot/css/dev.css` file and add the tailwind directives to it.

Add this to the `dev.css` file:
```postcss
   @tailwind base;
   @tailwind components;
   @tailwind utilities;
   ```

During development I like to run `npx tailwindcss -c ./tailwind.config.js -i wwwroot/css/dev.css -o wwwroot/css/app.css --watch`
to make the tailwindcss program watch for changes.


## Make the application full screen

We also need to make the application full screen. Otherwise, the bottom sheet will just be at the bottom of a very tiny
area.

Open the `layout/MainLayout.razor` and add these utility classes to the `main` element.
```html
<main class="flex flex-col min-h-screen">
    <!-- body content -->
</main>
```

## Create the UIBottomSheetProvider component
We're now ready to create the actual component.

Create a new blazor component here: `Components/BottomSheet/UIBottomSheetProvider.razor` and a corresponding code-behind
file `Components/BottomSheet/UIBottomSheetProvider.cs`. The `.razor` contains just our UI and the `.cs`

A bottom sheet typically consists of just a few visual elements: _the scrim_ which covers the full screen and fades the other UI.
The container, this is what you can consider the actual bottom sheet, and a drag handle.

Note that the scrim is only used when the bottom sheet is a "modal".

