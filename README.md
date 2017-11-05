# Kralizek.AspNetCore.Metrics

[![Build status](https://ci.appveyor.com/api/projects/status/24o7yhhr2sggarlv?svg=true)](https://ci.appveyor.com/project/Kralizek/aspnetcore-metrics)

## Project description

This projects aims to create a family of packages to quickly collect metrics regarding the performance of an ASP.NET Core site.

The main objective is to create components that are as little invasive as possible. Ideally, you only need to register the components in the `StartUp` class.

## Overview

Following ASP.NET Core vision, AspNetCore.Metrics is designed with an opt-in approach. You only need to use the packages that you want to use.

By default, it only collects metrics from the built-in ASP.NET Core pipeline. You can expand the amount of metrics collected by registering additional collectors. [ASP.NET Core MVC](https://www.nuget.org/packages/Kralizek.AspNetCore.Metrics.Collectors.Mvc/) and [AWS EC2](https://www.nuget.org/packages/Kralizek.AspNetCore.Metrics.Collectors.EC2/) are the only collectors available at the moment.

Once metrics are collected, they need to be pushed using a persister. A persister takes care of formatting the collected data following the format of a specific tool, be it [AWS CloudWatch](https://www.nuget.org/packages/Kralizek.AspNetCore.Metrics.CloudWatch/) or DataDog.

For a quick extensibility, all the interfaces are grouped in a package, [Kralizek.AspNetCore.Metrics.Abstractions](https://www.nuget.org/packages/Kralizek.AspNetCore.Metrics.Abstractions/). This makes it very easy to create additional collectors or persisters.

## Usage

## Downloads

Every library is available on [NuGet](https://www.nuget.org/packages?q=kralizek.aspnetcore.metrics) and is implemented according to the .NET Standard 2.0 specifications.

[Kralizek.AspNetCore.Metrics](https://www.nuget.org/packages/Kralizek.AspNetCore.Metrics/) contains the actual components to be attached to the ASP.NET Core pipeline.

[Kralizek.AspNetCore.Metrics.Abstractions](https://www.nuget.org/packages/Kralizek.AspNetCore.Metrics.Abstractions/) contains all the common types needed to create additional collectors or persisters.

[Kralizek.AspNetCore.Metrics.CloudWatch](https://www.nuget.org/packages/Kralizek.AspNetCore.Metrics.CloudWatch/) contains a persister that pushes all metrics into AWS CloudWatch.

[Kralizek.AspNetCore.Metrics.Collectors.EC2](https://www.nuget.org/packages/Kralizek.AspNetCore.Metrics.Collectors.EC2/) adds support for collecting information from the AWS EC2 environment, such as instance identifier, AMI identifier, availability zone and instance type.

[Kralizek.AspNetCore.Metrics.Collectors.Mvc](https://www.nuget.org/packages/Kralizek.AspNetCore.Metrics.Collectors.Mvc/) adds support for collecting information from the ASP.NET Core MVC pipeline, such as current controller and action name.