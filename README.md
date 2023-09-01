<h1 align="center">Tracking Chain</h1>
<div align="center">
<a href="https://github.com/TrackingChains/TrackingChain/actions/workflows/dotnet.yml"><img src="https://github.com/TrackingChains/TrackingChain/actions/workflows/dotnet.yml/badge.svg" alt=".NET"/></a>
<a href="https://github.com/TrackingChains/TrackingChain/actions/workflows/publish-stable.yml"><img src="https://github.com/TrackingChains/TrackingChain/actions/workflows/publish-stable.yml/badge.svg" alt="Publish stable release"/></a>
<a href="https://github.com/TrackingChains/TrackingChain/stargazers"><img src="https://img.shields.io/github/stars/TrackingChains/TrackingChain" alt="Stars Badge"/></a>
<a href="https://github.com/TrackingChains/TrackingChain/network/members"><img src="https://img.shields.io/github/forks/TrackingChains/TrackingChain" alt="Forks Badge"/></a>
<a href="https://github.com/TrackingChains/TrackingChain/pulls"><img src="https://img.shields.io/github/issues-pr/TrackingChains/TrackingChain" alt="Pull Requests Badge"/></a>
<a href="https://github.com/TrackingChains/TrackingChain/issues"><img src="https://img.shields.io/github/issues/TrackingChains/TrackingChain" alt="Issues Badge"/></a>
<a href="https://github.com/TrackingChains/TrackingChain/graphs/contributors"><img alt="GitHub contributors" src="https://img.shields.io/github/contributors/TrackingChains/TrackingChain?color=2b9348"></a>
<a href="https://github.com/TrackingChains/TrackingChain/blob/main/LICENSE"><img src="https://img.shields.io/github/license/TrackingChains/TrackingChain?color=2b9348" alt="License Badge"/></a>
</div>
<br>

  ![badge_black](https://github.com/TrackingChains/TrackingChain/assets/58514549/5d04542c-9adc-4e0d-9180-3b5985dcfd87)
  
## Project Overview :page_facing_up:

During this time, I have had the opportunity to work with several companies that wanted to adopt blockchain technology. However, I have observed that they often face challenges that hinder their adoption, mainly due to the following reasons:

 - Difficulties integrating legacy software with blockchain, such as dealing with long confirmation times or scalability issues when handling a large number of transactions. I have personally spoken with clients who need to handle over a million transactions per year, with peaks of thousands of requests per minute.

 - Concerns regarding wallet security, custody, accounting management, and the purchase of tokens for transaction fees.

 - Challenges in querying the blockchain to retrieve or interpret transactions over time, lacking a user-friendly interface.
 
 - High cost of using the blockchain (those who are not familiar with the blockchain world have heard of ethereum and how much it costs to do operations on it)

### Overview

To address these challenges, I have decided to create a web application specifically designed for companies and users who are eager to venture into the world of blockchain. The application aims to bridge the gap between "Web2" and "Web3" by providing a simple API call to feed data into smart contracts, with an immediate response providing a unique identifier.

To achieve this, I am developing a microservices architecture capable of handling millions of requests and scaling to accommodate peak traffic.

My plan involves creating an endpoint that can be accessed from Web2, exclusively responsible for collecting data values to be inserted into a smart contract. Currently, I am focusing on storing key-value pairs; however, I intend to dynamically handle more complex cases in the future. In this way, the Web2 user will be relieved of any concerns regarding the bottleneck presented by the blockchain, as their task will already be completed (which we address through our bridge development), we can manage an unlimited number of requests per second, ensuring a smooth user experience. Upon successful transaction completion, we will send a registration notification to the customer, including all relevant onchain transaction data. Additionally, we will provide a graphical tool enabling users to verify their transactions onchain, ensuring transparency and data correctness.

The application will handle all the necessary infrastructure setup for transaction transmission, including endpoint creation, failed transaction recovery, private key security, among others. The customer's role will be to select the appropriate smart contract type and chain for deployment, based on their future needs. For instance, in the future, certain data inputs may generate NFTs representing the final products, which could be utilized in other contexts through interoperability. Please note that this initial idea will not be present in the alpha version. Furthermore, we can leverage interoperability to store data in backup smart contracts created on secondary blockchains in case the primary chain faces congestion or other issues.

## Contents
  - [Wiki](https://github.com/TrackingChains/TrackingChain/wiki) for all info about the project.
  - [Configuration Step By Step](https://github.com/TrackingChains/TrackingChain/wiki/Configuration-Step-By-Step) for all configuration project.
  - [Demo Step By Step](https://github.com/TrackingChains/TrackingChain/wiki/Demo-Step-By-Step) for see a little demo of how project work.

# Credits

This project integrates the [SubstrateGaming](https://github.com/SubstrateGaming) library for the interface with the Substrate chains, I wanted to thank the team for the excellent work and the support offered for the realization of this project

# Contribute

Contributions are always welcome! Please create a PR to add Github Profile.

## :pencil: License

This project is licensed under [MIT](https://opensource.org/licenses/MIT) license.

## :man_astronaut: Show your support

Give a ⭐️ if this project helped you!
