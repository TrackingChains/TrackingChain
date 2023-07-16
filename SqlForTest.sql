
--https://blockscout.com/shibuya/address/0xa0532A56179Eb1677D33709db82de6b5880f23c6
--https://moonbase.moonscan.io/address/0x55D64aB19C01e135b86429D9367DfCEE3EF615a3#code

-- Astar Test
  insert into [Accounts] (Id, ChainWsAddress, ChainRpcAddress, PrivateKey) values
('4F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','wss://shibuya.blastapi.io/c8707e18-84fe-4b15-9665-a2897c0687df', 'https://shibuya.blastapi.io/c8707e18-84fe-4b15-9665-a2897c0687df', '4907f53615b76294a1238fbc9a9cd3d140beb9730b42085dc4c63e550a75357e')
  insert into [Accounts] (Id, ChainWsAddress, ChainRpcAddress, PrivateKey) values
('5F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', 'wss://shibuya.blastapi.io/7a2921d5-7c0c-411d-b687-4ba57cfbff25', 'https://shibuya.blastapi.io/7a2921d5-7c0c-411d-b687-4ba57cfbff25', '7f73040cc57fbcaa07ce6234cc6c153b982c9c73b5c9983aff0b108f99e6988d')
  insert into [Accounts] (Id, ChainWsAddress, ChainRpcAddress, PrivateKey) values
('6F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','wss://shibuya.blastapi.io/c8707e18-84fe-4b15-9665-a2897c0687df', 'https://shibuya.blastapi.io/c8707e18-84fe-4b15-9665-a2897c0687df', '21ff508926baa340d88a9157eeb03e969a676630951fa79e0d26a7a4a8288f68')
-- Moonbeam test
  insert into [Accounts] (Id, ChainWsAddress, ChainRpcAddress, PrivateKey) values
('8F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','wss://moonbase-alpha.blastapi.io/11cd5d86-565d-4b84-9cac-84cd08511215', 'https://moonbase-alpha.blastapi.io/11cd5d86-565d-4b84-9cac-84cd08511215', 'f60b7f53ae9593503174f55f5e236170d26c75e5a9beaf60c479b49563256a18')
  insert into [Accounts] (Id, ChainWsAddress, ChainRpcAddress, PrivateKey) values
('9F8E591B-6A9C-486E-AB2B-2B42ABBF5B23','wss://moonbase-alpha.blastapi.io/4e06bea6-56f2-40e0-a400-2e95f07a87e9', 'https://moonbase-alpha.blastapi.io/4e06bea6-56f2-40e0-a400-2e95f07a87e9', '7f4e1faf35cdbb5331fc52ed605ca3e9a341f65651c5be85651915be780b5c8f')

-- SmartContract
  insert into [SmartContract] (Address, ChainNumberId, ChainType, Currency, Name) values
('0xa0532A56179Eb1677D33709db82de6b5880f23c6', 81, 0, 'SBY', 'Shibuya Test Net')
insert into [SmartContract] (Address, ChainNumberId, ChainType, Currency, Name) values
('0x55D64aB19C01e135b86429D9367DfCEE3EF615a3', 1287, 0, 'DEV', 'Moonbase Test Net')

-- Profile
insert into [ProfileGroups] (Id, AggregationCode, Authority, Category, Name, SmartContractId, Priority) values
('8F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', NULL, NULL, 'Shibuya', 'Profilo Shibuya', 1, 0)
insert into [ProfileGroups] (Id, AggregationCode, Authority, Category, Name, SmartContractId, Priority) values
('DB909755-29D0-4AA4-8815-C77232743991', NULL, NULL, 'Moonbase', 'Profilo Moonbase', 2, 0)

-- Profile Shibuya
insert into [AccountProfileGroup] (AccountId, ProfileGroupId, Priority) values
('4F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', '8F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', 0)
insert into [AccountProfileGroup] (AccountId, ProfileGroupId, Priority) values
('5F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', '8F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', 0)
insert into [AccountProfileGroup] (AccountId, ProfileGroupId, Priority) values
('6F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', '8F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', 0)

-- Profile Moonbase
insert into [AccountProfileGroup] (AccountId, ProfileGroupId, Priority) values
('8F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', 'DB909755-29D0-4AA4-8815-C77232743991', 0)
insert into [AccountProfileGroup] (AccountId, ProfileGroupId, Priority) values
('9F8E591B-6A9C-486E-AB2B-2B42ABBF5B23', 'DB909755-29D0-4AA4-8815-C77232743991', 0)

