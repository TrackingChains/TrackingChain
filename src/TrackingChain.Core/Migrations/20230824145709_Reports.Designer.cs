﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TrackingChain.TrackingChainCore.EntityFramework.Context;

#nullable disable

namespace TrackingChain.Core.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230824145709_Reports")]
    partial class Reports
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TrackingChain.Core.Domain.Entities.Report", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<bool>("Reported")
                        .HasColumnType("bit");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Reports", (string)null);
                });

            modelBuilder.Entity("TrackingChain.TrackingChainCore.Domain.Entities.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ChainWatcherAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ChainWriterAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PrivateKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Accounts", (string)null);
                });

            modelBuilder.Entity("TrackingChain.TrackingChainCore.Domain.Entities.AccountProfileGroup", b =>
                {
                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ProfileGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.HasKey("AccountId", "ProfileGroupId");

                    b.HasIndex("ProfileGroupId");

                    b.ToTable("AccountProfileGroup");
                });

            modelBuilder.Entity("TrackingChain.TrackingChainCore.Domain.Entities.ProfileGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AggregationCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Authority")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<long>("SmartContractId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("SmartContractId");

                    b.ToTable("ProfileGroups", (string)null);
                });

            modelBuilder.Entity("TrackingChain.TrackingChainCore.Domain.Entities.SmartContract", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ChainNumberId")
                        .HasColumnType("int");

                    b.Property<int>("ChainType")
                        .HasColumnType("int");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExtraInfo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SmartContract", (string)null);
                });

            modelBuilder.Entity("TrackingChain.TrackingChainCore.Domain.Entities.TransactionPending", b =>
                {
                    b.Property<Guid>("TrackingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ChainNumberId")
                        .HasColumnType("int");

                    b.Property<int>("ChainType")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Completed")
                        .HasColumnType("bit");

                    b.Property<string>("DataValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ErrorTimes")
                        .HasColumnType("int");

                    b.Property<bool>("IsInProgress")
                        .HasColumnType("bit");

                    b.Property<int?>("LastUnlockedError")
                        .HasColumnType("int");

                    b.Property<bool>("Locked")
                        .IsConcurrencyToken()
                        .HasColumnType("bit");

                    b.Property<Guid?>("LockedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LockedDated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("PoolDate")
                        .HasColumnType("datetime2");

                    b.Property<byte>("Priority")
                        .HasColumnType("tinyint");

                    b.Property<Guid>("ProfileGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ReceivedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SmartContractAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SmartContractExtraInfo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("SmartContractId")
                        .HasColumnType("bigint");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("TriageDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TxHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("WatchingFrom")
                        .HasColumnType("datetime2");

                    b.HasKey("TrackingId");

                    b.ToTable("TransactionPendings", (string)null);
                });

            modelBuilder.Entity("TrackingChain.TrackingChainCore.Domain.Entities.TransactionPool", b =>
                {
                    b.Property<Guid>("TrackingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ChainNumberId")
                        .HasColumnType("int");

                    b.Property<int>("ChainType")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Completed")
                        .HasColumnType("bit");

                    b.Property<string>("DataValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ErrorTimes")
                        .HasColumnType("int");

                    b.Property<DateTime>("GeneratingFrom")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Locked")
                        .IsConcurrencyToken()
                        .HasColumnType("bit");

                    b.Property<Guid?>("LockedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("LockedDated")
                        .HasColumnType("datetime2");

                    b.Property<byte>("Priority")
                        .HasColumnType("tinyint");

                    b.Property<Guid>("ProfileGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ReceivedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SmartContractAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SmartContractExtraInfo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("SmartContractId")
                        .HasColumnType("bigint");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("TriageDate")
                        .HasColumnType("datetime2");

                    b.HasKey("TrackingId");

                    b.HasIndex("Locked", "Priority");

                    b.ToTable("TransactionPools", (string)null);
                });

            modelBuilder.Entity("TrackingChain.TrackingChainCore.Domain.Entities.TransactionRegistry", b =>
                {
                    b.Property<Guid>("TrackingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ChainNumberId")
                        .HasColumnType("int");

                    b.Property<int>("ChainType")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DataValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ErrorTime")
                        .HasColumnType("int");

                    b.Property<string>("LastTransactionHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PendingDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("PoolDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ProfileGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ReceiptBlockHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReceiptBlockNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReceiptCumulativeGasUsed")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReceiptEffectiveGasPrice")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReceiptFrom")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReceiptGasUsed")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ReceiptReceived")
                        .HasColumnType("bit");

                    b.Property<string>("ReceiptTo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReceiptTransactionHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ReceivedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("RegistryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SmartContractAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SmartContractEndpoint")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SmartContractExtraInfo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("SmartContractId")
                        .HasColumnType("bigint");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int?>("TransactionErrorReason")
                        .HasColumnType("int");

                    b.Property<int>("TransactionStep")
                        .HasColumnType("int");

                    b.Property<DateTime>("TriageDate")
                        .HasColumnType("datetime2");

                    b.HasKey("TrackingId");

                    b.ToTable("TransactionRegistries", (string)null);
                });

            modelBuilder.Entity("TrackingChain.TrackingChainCore.Domain.Entities.TransactionTriage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("ChainNumberId")
                        .HasColumnType("int");

                    b.Property<int>("ChainType")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Completed")
                        .HasColumnType("bit");

                    b.Property<string>("DataValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsInPool")
                        .HasColumnType("bit");

                    b.Property<Guid>("ProfileGroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ReceivedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("SmartContractAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SmartContractExtraInfo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("SmartContractId")
                        .HasColumnType("bigint");

                    b.Property<Guid>("TrackingIdentify")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("IsInPool", "Completed");

                    b.ToTable("TransactionTriages", (string)null);
                });

            modelBuilder.Entity("TrackingChain.TrackingChainCore.Domain.Entities.AccountProfileGroup", b =>
                {
                    b.HasOne("TrackingChain.TrackingChainCore.Domain.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TrackingChain.TrackingChainCore.Domain.Entities.ProfileGroup", "ProfileGroup")
                        .WithMany()
                        .HasForeignKey("ProfileGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("ProfileGroup");
                });

            modelBuilder.Entity("TrackingChain.TrackingChainCore.Domain.Entities.ProfileGroup", b =>
                {
                    b.HasOne("TrackingChain.TrackingChainCore.Domain.Entities.SmartContract", "SmartContract")
                        .WithMany("ProfileGroups")
                        .HasForeignKey("SmartContractId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SmartContract");
                });

            modelBuilder.Entity("TrackingChain.TrackingChainCore.Domain.Entities.SmartContract", b =>
                {
                    b.Navigation("ProfileGroups");
                });
#pragma warning restore 612, 618
        }
    }
}
