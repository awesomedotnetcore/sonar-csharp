/*
 * Sonar C# Plugin :: Gallio
 * Copyright (C) 2010 Jose Chillan, Alexandre Victoor and SonarSource
 * dev@sonar.codehaus.org
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02
 */
package org.sonar.plugins.csharp.gallio;

import java.util.ArrayList;
import java.util.List;

import org.sonar.api.Extension;
import org.sonar.api.Properties;
import org.sonar.api.Property;
import org.sonar.api.SonarPlugin;

/**
 * C# Gallio plugin class.
 */
@Properties({
    @Property(key = GallioConstants.INSTALL_FOLDER_KEY, defaultValue = GallioConstants.INSTALL_FOLDER_DEFVALUE,
        name = "Gallio install directory", description = "Absolute path of the Gallio installation folder.", global = true, project = false),
    @Property(
        key = GallioConstants.FILTER_KEY,
        defaultValue = GallioConstants.FILTER_DEFVALUE,
        name = "Test filter",
        description = "Filter that can be used to execute only a specific test category (i.e. CategotyName:unit to consider only tests from the 'unit' category).",
        global = true, project = true),
    @Property(key = GallioConstants.TIMEOUT_MINUTES_KEY, defaultValue = GallioConstants.TIMEOUT_MINUTES_DEFVALUE + "",
        name = "Gallio program timeout", description = "Maximum number of minutes before the Gallio program will be stopped.",
        global = true, project = true),
    @Property(key = GallioConstants.MODE, defaultValue = "", name = "Gallio activation mode",
        description = "Possible values : empty (means active), 'skip' and 'reuseReport'.", global = true, project = true),
    @Property(key = GallioConstants.REPORTS_PATH_KEY, defaultValue = "", name = "Name of the Gallio report files",
        description = "Name of the Gallio report file used when reuse report mode is activated.", global = true, project = true) })
public class GallioPlugin extends SonarPlugin {

  /**
   * {@inheritDoc}
   */
  public List<Class<? extends Extension>> getExtensions() {
    List<Class<? extends Extension>> extensions = new ArrayList<Class<? extends Extension>>();
    extensions.add(GallioMetrics.class);
    extensions.add(GallioExecutionSensor.class);
    extensions.add(GallioTestSensor.class);

    return extensions;
  }
}