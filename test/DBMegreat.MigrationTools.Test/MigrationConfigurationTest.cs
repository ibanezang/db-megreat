using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace DBMegreat.MigrationTools.Test
{
    public class MigrationConfigurationTest
    {
        [Fact]
        public void ParseConfiguration_WithValidConfigurationJson_CreatesCorrectConfigurationObject()
        {
            var expected = new MigrationConfiguration
            {
                SqlFilesDirectories = new List<string> { "/your/directory/contains/sql", "../another/directory/contains/sql" },
                DbConnection = new ConnectionConfiguration
                {
                    Type = SqlType.MySql,
                    ConnectionString = "Server=HOST_NAME;Database=DB_NAME;Uid=USER_ID;Pwd=PASSWORD"
                },
                LogOutputDirectory = "../directory/output"
            };

            var json = @"
                {
                    ""sql_files_directories"": [
                        ""/your/directory/contains/sql"",
                        ""../another/directory/contains/sql""
                    ],
                    ""db_connection"": {
                        ""type"": ""mysql"",
                        ""connection_string"": ""Server=HOST_NAME;Database=DB_NAME;Uid=USER_ID;Pwd=PASSWORD""
                    },
                    ""log_output_directory"": ""../directory/output""
                }
            ";
            var configuration = MigrationConfiguration.ParseConfiguration(json);

            configuration.SqlFilesDirectories.Count.ShouldBe(expected.SqlFilesDirectories.Count);
            foreach (var directory in configuration.SqlFilesDirectories)
            {
                expected.SqlFilesDirectories.ShouldContain(directory);
            }

            configuration.DbConnection.Type.ShouldBe(expected.DbConnection.Type);
            configuration.DbConnection.ConnectionString.ShouldBe(expected.DbConnection.ConnectionString);

            configuration.LogOutputDirectory.ShouldBe(expected.LogOutputDirectory);
        }

        [Fact]
        public void ParseConfiguration_WithNoLogOutput_CreatesCorrectConfigurationObject()
        {
            var expected = new MigrationConfiguration
            {
                SqlFilesDirectories = new List<string> { "/your/directory/contains/sql", "../another/directory/contains/sql" },
                DbConnection = new ConnectionConfiguration
                {
                    Type = SqlType.MySql,
                    ConnectionString = "Server=HOST_NAME;Database=DB_NAME;Uid=USER_ID;Pwd=PASSWORD"
                }
            };

            var json = @"
                {
                    ""sql_files_directories"": [
                        ""/your/directory/contains/sql"",
                        ""../another/directory/contains/sql""
                    ],
                    ""db_connection"": {
                        ""type"": ""mysql"",
                        ""connection_string"": ""Server=HOST_NAME;Database=DB_NAME;Uid=USER_ID;Pwd=PASSWORD""
                    }
                }
            ";
            var configuration = MigrationConfiguration.ParseConfiguration(json);

            configuration.SqlFilesDirectories.Count.ShouldBe(expected.SqlFilesDirectories.Count);
            foreach (var directory in configuration.SqlFilesDirectories)
            {
                expected.SqlFilesDirectories.ShouldContain(directory);
            }

            configuration.DbConnection.Type.ShouldBe(expected.DbConnection.Type);
            configuration.DbConnection.ConnectionString.ShouldBe(expected.DbConnection.ConnectionString);

            configuration.LogOutputDirectory.ShouldBe(expected.LogOutputDirectory);
        }

        [Fact]
        public void ParseConfiguration_WithInvalidConfigurationJson_ShouldThrowInvalidConfigurationException()
        {
            var exception = Should.Throw<InvalidConfigurationException>(() => MigrationConfiguration.ParseConfiguration("invalid configuration"));
            exception.Message.ShouldBe("Invalid configuration file.");
        }

        [Fact]
        public void ParseConfiguration_WithNoSqlFilesDirectories_ShouldThrowInvalidConfigurationException()
        {
            var json = @"
                {
                    ""db_connection"": {
                        ""type"": ""mysql"",
                        ""connection_string"": ""Server=HOST_NAME;Database=DB_NAME;Uid=USER_ID;Pwd=PASSWORD""
                    },
                    ""log_output"": ""../directory/output""
                }
            ";
            var exception = Should.Throw<InvalidConfigurationException>(() => MigrationConfiguration.ParseConfiguration(json));
            exception.Message.ShouldBe("sql_files_directories configuration was not found.");
        }

        [Fact]
        public void ParseConfiguration_WithNoDBConnection_ShouldThrowInvalidConfigurationException()
        {
            var json = @"
                {
                    ""sql_files_directories"": [
                        ""/your/directory/contains/sql"",
                        ""../another/directory/contains/sql""
                    ],
                    ""log_output"": ""../directory/output""
                }
            ";
            var exception = Should.Throw<InvalidConfigurationException>(() => MigrationConfiguration.ParseConfiguration(json));
            exception.Message.ShouldBe("db_connection configuration was not found.");
        }

        [Fact]
        public void ParseConfiguration_WithConnectionString_ShouldThrowInvalidConfigurationException()
        {
            var json = @"
                {
                    ""sql_files_directories"": [
                        ""/your/directory/contains/sql"",
                        ""../another/directory/contains/sql""
                    ],
                    ""db_connection"": {
                        ""type"": ""mysql""
                    },
                    ""log_output"": ""../directory/output""
                }
            ";
            var exception = Should.Throw<InvalidConfigurationException>(() => MigrationConfiguration.ParseConfiguration(json));
            exception.Message.ShouldBe("db_connection.connection_string configuration was not found or has invalid value.");
        }

        [Fact]
        public void ParseConfiguration_WithEmptySqlDirectories_ShouldThrowInvalidConfigurationException()
        {
            var json = @"
                {
                    ""sql_files_directories"": [
                    ],
                    ""db_connection"": {
                        ""type"": ""mysql"",
                        ""connection_string"": ""Server=HOST_NAME;Database=DB_NAME;Uid=USER_ID;Pwd=PASSWORD""
                    },
                    ""log_output"": ""../directory/output""
                }
            ";
            var exception = Should.Throw<InvalidConfigurationException>(() => MigrationConfiguration.ParseConfiguration(json));
            exception.Message.ShouldBe("sql_files_directories configuration was empty.");

        }

        [Fact]
        public void ParseConfiguration_WithInvalidSqlDirectoriesValue_ShouldThrowInvalidConfigurationException()
        {
            var json = @"
                {
                    ""sql_files_directories"": [
                        ""/your/directory/contains/sql"",
                        """"
                    ],
                    ""db_connection"": {
                        ""type"": ""mysql"",
                        ""connection_string"": ""Server=HOST_NAME;Database=DB_NAME;Uid=USER_ID;Pwd=PASSWORD""
                    },
                    ""log_output"": ""../directory/output""
                }
            ";
            var exception = Should.Throw<InvalidConfigurationException>(() => MigrationConfiguration.ParseConfiguration(json));
            exception.Message.ShouldBe("sql_files_directories has invalid value.");
        }
    }
}
