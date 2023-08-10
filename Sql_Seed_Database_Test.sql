-- Astar Test EVM
insert into [Accounts] (Id, ChainWriterAddress, ChainWatcherAddress, Name, PrivateKey) values
('4F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','https://shibuya.blastapi.io/c8707e18-84fe-4b15-9665-a2897c0687df', 'https://shibuya.blastapi.io/c8707e18-84fe-4b15-9665-a2897c0687df', 'Astar Test EVM One', '.....................')
insert into [Accounts] (Id, ChainWriterAddress, ChainWatcherAddress, Name, PrivateKey) values
('5F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', 'https://shibuya.blastapi.io/7a2921d5-7c0c-411d-b687-4ba57cfbff25', 'https://shibuya.blastapi.io/7a2921d5-7c0c-411d-b687-4ba57cfbff25', 'Astar Test EVM Two', '.....................')
insert into [Accounts] (Id, ChainWriterAddress, ChainWatcherAddress, Name, PrivateKey) values
('6F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','https://shibuya.blastapi.io/c8707e18-84fe-4b15-9665-a2897c0687df', 'https://shibuya.blastapi.io/c8707e18-84fe-4b15-9665-a2897c0687df', 'Astar Test EVM Three', '.....................')
-- Moonbeam Test EVM
insert into [Accounts] (Id, ChainWriterAddress, ChainWatcherAddress, Name, PrivateKey) values
('8F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','https://moonbase-alpha.blastapi.io/11cd5d86-565d-4b84-9cac-84cd08511215', 'https://moonbase-alpha.blastapi.io/11cd5d86-565d-4b84-9cac-84cd08511215', 'Moonbeam Test EVM One', '.....................')
insert into [Accounts] (Id, ChainWriterAddress, ChainWatcherAddress, Name, PrivateKey) values
('9F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','https://moonbase-alpha.blastapi.io/4e06bea6-56f2-40e0-a400-2e95f07a87e9', 'https://moonbase-alpha.blastapi.io/4e06bea6-56f2-40e0-a400-2e95f07a87e9', 'Moonbeam Test EVM Two', '.....................')
-- Astar Test Ink
insert into [Accounts] (Id, ChainWriterAddress, ChainWatcherAddress, Name, PrivateKey) values
('1F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','wss://shibuya.blastapi.io/c8707e18-84fe-4b15-9665-a2897c0687df', 'https://shibuya.api.subscan.io|c5b981ec4efa4ad69a39578e81f1d583', 'Astar Test Ink', '.....................')
-- Contract Rococo Test Ink
insert into [Accounts] (Id, ChainWriterAddress, ChainWatcherAddress, Name, PrivateKey) values
('2F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','wss://rococo-contracts-rpc.polkadot.io', '', 'Rococo Test Ink','.....................')

-- SmartContract
insert into [SmartContract] (Address, ChainNumberId, ChainType, Currency, ExtraInfo, Name) values
('0xa0532A56179Eb1677D33709db82de6b5880f23c6', 81, 0, 'SBY', '{"WaitingSecondsForWatcherTx":90}', 'Shibuya Test Net EVM')
insert into [SmartContract] (Address, ChainNumberId, ChainType, Currency, ExtraInfo, Name) values
('0x55D64aB19C01e135b86429D9367DfCEE3EF615a3', 1287, 0, 'DEV', '{"WaitingSecondsForWatcherTx":90}', 'Moonbase Test Net')
insert into [SmartContract] (Address, ChainNumberId, ChainType, Currency, ExtraInfo, Name) values
('XqLt7FSZrn8nffSGkRYWZKn4JCWZBvHNH3sTRnQL4qDr2Dp', 101, 1, 'SBY', '{"InsertTrackSelectorValue":"0x1ba63d86","ProofSize":125952,"RefTime":4793859072,"BasicWeight":49000000000,"ByteWeight":1000000000,"SupportedClient":0,"WaitingSecondsForWatcherTx":90}', 'Shibuya Test Net INK')
insert into [SmartContract] (Address, ChainNumberId, ChainType, Currency, ExtraInfo, Name) values
('5C557tarfZcHfxAeCLd67wNjuQCiuvaJr23Qd56kPjUxn382', 9420, 1, 'ROC', '{"InsertTrackSelectorValue":"0x1ba63d86","ProofSize":125952,"RefTime":4793859072,"BasicWeight":6682666644,"ByteWeight":333333,"SupportedClient":1,"WaitingSecondsForWatcherTx":90}', 'Contract Rococo Test Net INK')

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
