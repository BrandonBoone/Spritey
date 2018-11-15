<h1 align="center">
    <img src="https://raw.githubusercontent.com/BrandonBoone/Spritey/master/docs/Spritey3.png" alt="Spritey" width="256" />
    <br />
</h1>
<div align="center">

[![GitHub license](https://img.shields.io/badge/license-MIT%20%26%20CPOL%201.02-blue.svg)](https://raw.githubusercontent.com/BrandonBoone/Spritey/master/LICENSE)
[![Twitter](https://img.shields.io/twitter/url/http/shields.io.svg?style=flat&logo=twitter)](https://twitter.com/intent/tweet?hashtags=spritey,dotnet,oss&text=Spritey.+A+new+sprite+generator+in+C%23&url=https%3a%2f%2fgithub.com%2fBrandonBoone%2fSpritey&via=brandonjboone)

</div>

While sprites may not be in vogue anymore and ~~font~~ svg based icon libraries are all the rage, css based sprites still have their place (*i think*), so why not implement yet another sprite generator in .NET Core.

Spritey is a sprite generator. It takes a directory of images and converts them to an optimally packed rectangle, outputting a css embedded or referenced `gif` or `png` as well as the accompanying css stylesheet.

While the generated `gif` is substantially smaller than the `png`, it suffers from quality degradation, especially if the source images vary in style (colors / complexity). The output should always be checked for quality.

### Goal

The end goal of this project is to produce both a .NET Core NuGet library for consuming apps, as well as a .NET Core Global Tool for CLI sprite generation.

### Build Status

|             |Build Status|Code Coverage|Quality|
|-------------|:----------:|:-----------:|:-----------:|
|**Windows & Linux**  |[![Build Status](https://dev.azure.com/Spritey/Spritey/_apis/build/status/Spritey)](https://dev.azure.com/Spritey/Spritey/_build/latest?definitionId=1)|[![codecov](https://codecov.io/gh/BrandonBoone/Spritey/branch/master/graph/badge.svg)](https://codecov.io/gh/BrandonBoone/Spritey)|[![sonarcloud](https://sonarcloud.io/api/project_badges/measure?project=BrandonBoone_Spritey&metric=alert_status)](https://sonarcloud.io/dashboard?id=BrandonBoone_Spritey)|

### API

#### Option 1

- generate css with gif or png that is embedded or referenced

```csharp
using Spritey;

string directoryOfImages = ""; // path to your images
string outputDirectory = ""; // path to output files

using (Sprite sprite = new Sprite(directoryOfImages))
{
    // css with referenced gif
    sprite.Save("spriteName", outputDirectory, false, SpriteFormat.Gif);
}

```

#### Option 2

- generate css with embedded or referenced gif
- and generate css with embedded or referenced png

```csharp
using Spritey;

string directoryOfImages = ""; // path to your images
string outputDirectory = ""; // path to output files

using (Sprite sprite = new Sprite(directoryOfImages))
{
    // css with embedded gif and css with embedded png
    sprite.Save("spriteName", outputDirectory, true);
}

```

#### Option 3

- generate css with embedded gif
- and generate css with embedded png
- and generate css and referenced gif
- and generate css and referenced png

```csharp
using Spritey;

string directoryOfImages = ""; // path to your images
string outputDirectory = ""; // path to output files

using (Sprite sprite = new Sprite(directoryOfImages))
{
    // output all variations
    sprite.Save("spriteName", outputDirectory);
}
```

### Getting started

#### Building in VSCode

- Build: `ctrl + p => task build`
- Test: `ctrl + p => task test`
