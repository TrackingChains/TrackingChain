
--https://blockscout.com/shibuya/address/0xa0532A56179Eb1677D33709db82de6b5880f23c6
--https://moonbase.moonscan.io/address/0x55D64aB19C01e135b86429D9367DfCEE3EF615a3#code
--https://shibuya.subscan.io/account/aKpb5m5WBvTA164EdZhkYHU1SHBixY4QPnxbekMDUSfUYGd

-- Astar Test EVM
insert into [Accounts] (Id, ChainWriterAddress, ChainWatcherAddress, PrivateKey) values
('4F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','https://shibuya.blastapi.io/c8707e18-84fe-4b15-9665-a2897c0687df', 'https://shibuya.blastapi.io/c8707e18-84fe-4b15-9665-a2897c0687df', '.........')
insert into [Accounts] (Id, ChainWriterAddress, ChainWatcherAddress, PrivateKey) values
('5F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', 'https://shibuya.blastapi.io/7a2921d5-7c0c-411d-b687-4ba57cfbff25', 'https://shibuya.blastapi.io/7a2921d5-7c0c-411d-b687-4ba57cfbff25', '.........')
insert into [Accounts] (Id, ChainWriterAddress, ChainWatcherAddress, PrivateKey) values
('6F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','https://shibuya.blastapi.io/c8707e18-84fe-4b15-9665-a2897c0687df', 'https://shibuya.blastapi.io/c8707e18-84fe-4b15-9665-a2897c0687df', '.........')
-- Moonbeam Test EVM
insert into [Accounts] (Id, ChainWriterAddress, ChainWatcherAddress, PrivateKey) values
('8F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','https://moonbase-alpha.blastapi.io/11cd5d86-565d-4b84-9cac-84cd08511215', 'https://moonbase-alpha.blastapi.io/11cd5d86-565d-4b84-9cac-84cd08511215', '.........')
insert into [Accounts] (Id, ChainWriterAddress, ChainWatcherAddress, PrivateKey) values
('9F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','https://moonbase-alpha.blastapi.io/4e06bea6-56f2-40e0-a400-2e95f07a87e9', 'https://moonbase-alpha.blastapi.io/4e06bea6-56f2-40e0-a400-2e95f07a87e9', '.........')
-- Astar Test Ink
insert into [Accounts] (Id, ChainWriterAddress, ChainWatcherAddress, PrivateKey) values
('1F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','wss://shibuya.blastapi.io/c8707e18-84fe-4b15-9665-a2897c0687df', 'https://shibuya.api.subscan.io|c5b981ec4efa4ad69a39578e81f1d583', '.........')
-- Contract Rococo Test Ink
insert into [Accounts] (Id, ChainWriterAddress, ChainWatcherAddress, PrivateKey) values
('2F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','wss://rococo-contracts-rpc.polkadot.io', '', '.........')

-- SmartContract
insert into [SmartContract] (Address, ChainNumberId, ChainType, Currency, ExtraInfo, Name) values
('0xa0532A56179Eb1677D33709db82de6b5880f23c6', 81, 0, 'SBY', '{}', 'Shibuya Test Net EVM')
insert into [SmartContract] (Address, ChainNumberId, ChainType, Currency, ExtraInfo, Name) values
('0x55D64aB19C01e135b86429D9367DfCEE3EF615a3', 1287, 0, 'DEV', '{}', 'Moonbase Test Net')
insert into [SmartContract] (Address, ChainNumberId, ChainType, Currency, ExtraInfo, Name) values
('Yo6k7mxeRSK1nA2xrXEXwBruhcqbiMhLVbn3GPyoQUS2vwp', 101, 1, 'SBY', '{"InsertTrackSelectorValue":"0x1ba63d86","ProofSize":125952,"RefTime":4793859072,"BasicWeight":14000000000,"ByteWeight":1000000000,"SupportedClient":0}', 'Shibuya Test Net INK')
insert into [SmartContract] (Address, ChainNumberId, ChainType, Currency, ExtraInfo, Name) values
('5CiSb6GG9mtpBSDoS52vrQjhmYaRLVKYE7V8rNducjLXma5T', 9420, 1, 'ROC', '{"InsertTrackSelectorValue":"0x1ba63d86","ProofSize":125952,"RefTime":4793859072,"BasicWeight":4999995,"ByteWeight":333333,"SupportedClient":1}', 'Contract Rococo Test Net INK')

-- Profile
insert into [ProfileGroups] (Id, AggregationCode, Authority, Category, Name, SmartContractId, Priority) values
('8F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', NULL, NULL, 'Shibuya EVM', 'EVM Solidity Shibuya', 1, 0)
insert into [ProfileGroups] (Id, AggregationCode, Authority, Category, Name, SmartContractId, Priority) values
('DB909755-29D0-4AA4-8815-C77232743991', NULL, NULL, 'Moonbase EVM', 'EVM Solidity Moonbase', 2, 0)
insert into [ProfileGroups] (Id, AggregationCode, Authority, Category, Name, SmartContractId, Priority) values
('71D944E7-CC29-4CA2-9B4D-78A07A503A52', NULL, NULL, 'Shibuya Ink', 'Substrate Ink Shibuya', 3, 0)
insert into [ProfileGroups] (Id, AggregationCode, Authority, Category, Name, SmartContractId, Priority) values
('51D944E7-CC29-4CA2-9B4D-78A07A503A52', NULL, NULL, 'Rococo Ink', 'Substrate Ink Contract Rococo', 4, 0)


-- Profile Shibuya EVM
insert into [AccountProfileGroup] (AccountId, ProfileGroupId, Priority) values
('4F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', '8F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', 0)
insert into [AccountProfileGroup] (AccountId, ProfileGroupId, Priority) values
('5F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', '8F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', 0)
insert into [AccountProfileGroup] (AccountId, ProfileGroupId, Priority) values
('6F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', '8F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', 0)

-- Profile Moonbase EVM
insert into [AccountProfileGroup] (AccountId, ProfileGroupId, Priority) values
('8F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', 'DB909755-29D0-4AA4-8815-C77232743991', 0)
insert into [AccountProfileGroup] (AccountId, ProfileGroupId, Priority) values
('9F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', 'DB909755-29D0-4AA4-8815-C77232743991', 0)

-- Profile Shibuya Ink
insert into [AccountProfileGroup] (AccountId, ProfileGroupId, Priority) values
('1F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', '71D944E7-CC29-4CA2-9B4D-78A07A503A52', 0)

-- Contract Rococo Ink
insert into [AccountProfileGroup] (AccountId, ProfileGroupId, Priority) values
('2F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', '51D944E7-CC29-4CA2-9B4D-78A07A503A52', 0)
