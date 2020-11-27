import React from 'react';
import { isExperienceEditorActive, Placeholder, Image } from '@sitecore-jss/sitecore-jss-react';

const SnippetComponent = (props) => {
  
  if (!props.fields) {
    if (isExperienceEditorActive()) {
      return <h1 className="alarm">Datasource isn't set.</h1>;
    }
    return (
      <h1 className="alarm">
        Data is not provided. Contact administrators, please.
      </h1>
    );
  }

 return( 
      <>
        <Placeholder name="snippet-content" rendering={props.rendering} />  
      </>
);
}
export default SnippetComponent;

