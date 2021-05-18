
// https://www.enchantedlearning.com/wordlist
(function() {
    const words = Array.from(document.querySelectorAll('.wordlist-section .wordlist-item')).map(x => x.innerText);
    const wordMdFormat = words.map(word => {
        // capitalize first letter
        // word = word[0].toUpperCase() + word.slice(1);
        return `***\n${word}\n\n\n---\n\n`;
    }).join('');
    console.log(wordMdFormat);
})()
