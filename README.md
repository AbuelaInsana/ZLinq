# ZLinq 🌟

![ZLinq](https://img.shields.io/badge/ZLinq-v1.0.0-blue.svg)  
[![Releases](https://img.shields.io/badge/Releases-Check%20Here-orange.svg)](https://github.com/AbuelaInsana/ZLinq/releases)

Welcome to **ZLinq**, your go-to solution for zero allocation LINQ in .NET environments. This library provides powerful features, including LINQ to Span, LINQ to SIMD, and LINQ to Tree for various applications, such as FileSystem, JSON handling, and GameObject management in both Unity and Godot.

## Table of Contents

1. [Features](#features)
2. [Installation](#installation)
3. [Usage](#usage)
4. [Examples](#examples)
5. [Contributing](#contributing)
6. [License](#license)
7. [Support](#support)

## Features

- **Zero Allocation LINQ**: Designed to minimize memory allocations, ensuring efficient performance.
- **LINQ to Span**: Work with slices of arrays and other data structures seamlessly.
- **LINQ to SIMD**: Utilize Single Instruction, Multiple Data (SIMD) for high-performance data processing.
- **LINQ to Tree**: Support for various data types, including FileSystem structures, JSON data, and GameObjects.
- **Cross-Platform Compatibility**: Works across all .NET platforms, Unity, and Godot.

## Installation

To get started with ZLinq, you can download the latest release from the [Releases section](https://github.com/AbuelaInsana/ZLinq/releases). Follow the instructions in the release notes to execute the package correctly.

### Using NuGet

You can also install ZLinq via NuGet. Run the following command in your package manager console:

```bash
Install-Package ZLinq
```

This will add ZLinq to your project and make it ready for use.

## Usage

Once you have ZLinq installed, you can start using it in your project. Here’s a simple example of how to use ZLinq in a C# application:

```csharp
using ZLinq;

class Program
{
    static void Main()
    {
        var numbers = new Span<int>(new[] { 1, 2, 3, 4, 5 });
        
        var result = numbers.Select(n => n * 2).ToArray();
        
        Console.WriteLine(string.Join(", ", result)); // Output: 2, 4, 6, 8, 10
    }
}
```

This example demonstrates how to utilize LINQ to Span for efficient array manipulation.

## Examples

### LINQ to SIMD

Here’s an example of using LINQ to SIMD for vector operations:

```csharp
using ZLinq;

class SIMDExample
{
    static void Main()
    {
        var vectorA = new Span<float>(new[] { 1f, 2f, 3f, 4f });
        var vectorB = new Span<float>(new[] { 5f, 6f, 7f, 8f });

        var result = vectorA.Add(vectorB);

        Console.WriteLine(string.Join(", ", result)); // Output: 6, 8, 10, 12
    }
}
```

### LINQ to Tree

You can also work with hierarchical data structures using LINQ to Tree. Here’s an example of querying a JSON object:

```csharp
using ZLinq;
using Newtonsoft.Json;

class TreeExample
{
    static void Main()
    {
        var json = "{\"name\":\"John\", \"age\":30, \"children\":[{\"name\":\"Jane\", \"age\":10}]}";
        var obj = JsonConvert.DeserializeObject<dynamic>(json);

        var childrenNames = obj.children.Select(c => c.name);
        
        Console.WriteLine(string.Join(", ", childrenNames)); // Output: Jane
    }
}
```

## Contributing

We welcome contributions to ZLinq. If you would like to help improve this library, please follow these steps:

1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Make your changes and commit them.
4. Push your branch to your fork.
5. Create a pull request.

Please ensure that your code adheres to our coding standards and includes appropriate tests.

## License

ZLinq is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## Support

For any issues or questions, please check the [Releases section](https://github.com/AbuelaInsana/ZLinq/releases) for updates. If you need further assistance, feel free to open an issue in the repository.

---

Thank you for choosing ZLinq! We hope it enhances your development experience.